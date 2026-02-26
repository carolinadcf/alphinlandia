# Page-Turn Animation

## Context

When the player clicks a page, the book plays a physical page-turn animation using DOTween. A page lifts off the right side, sweeps over the spine, and lands on the left — revealing the next spread's content during the animation rather than swapping it instantly.

The animation requires 5 page GameObjects in total: 2 static (LeftPage, RightPage) and 3 for the turning page (TurningPagePivot + TurningFront + TurningBack). Because all quads are single-sided (back-face culling on), TurningFront is naturally visible in the first half of the sweep and TurningBack in the second half — no extra visibility logic needed.

---

## Book Prefab Hierarchy

```
Book
├── LeftPage              static, always visible
├── RightPage             static, always visible
└── TurningPagePivot      empty GO at the spine — rotates during animation
    ├── TurningFront      shows the page being turned away (first half)
    └── TurningBack       shows the arriving page (second half)
```

### TurningPagePivot

Empty GameObject. Its only role is to be the rotation pivot at the book spine.

| Property | Value |
|----------|-------|
| Local Position | (2.35, 0.5, 0) — midpoint between LeftPage (x=2.0) and RightPage (x=2.7) |
| Local Rotation | (0, 0, 0) at rest — animated to (0, -180, 0) during a turn |
| Local Scale | (1, 1, 1) |
| Active at rest | **false** — activated only during animation |

### TurningFront / TurningBack

Both are Quad children of TurningPagePivot with the same position and scale. They face opposite directions via their Y rotation, making them mutually exclusive in view.

| Property | TurningFront | TurningBack |
|----------|-------------|------------|
| Local Position | (0.35, 0, 0)* | (0.35, 0, 0)* |
| Local Rotation | (90, 180, 0) | (90, 0, 0) |
| Local Scale | (0.07, 0.1, 0.1) | (0.07, 0.1, 0.1) |
| Material | `TurningFrontPage.mat` | `TurningBackPage.mat` |
| MeshCollider | None | None |
| Child | StoryText (TMP) | StoryText (TMP) |

> *The X offset places the page center 0.35 units from the spine (matching the right page position). Verify the sign in the editor — flip to -0.35 if the quad appears on the wrong side.*

**Materials:** `TurningFrontPage.mat` and `TurningBackPage.mat` are duplicates of `BookPage.mat`. Separate instances are required so `SetPageContent` doesn't mutate the static pages' shared material.

---

## Animation Flow

| Phase | Pivot Y rotation | What the player sees |
|-------|-----------------|----------------------|
| Start | 0° | TurningFront (current right page) lifts off and sweeps toward vertical |
| Midpoint | -90° (edge-on) | Page is invisible — static LeftPage content swaps silently |
| Second half | -90° → -180° | TurningBack (next left page) sweeps down; new RightPage already visible underneath |
| End | -180° | TurningPagePivot deactivated; static spread shows next content |

---

## BookManager Changes

### New fields

```csharp
[Header("Turning Page")]
[SerializeField] private GameObject turningPagePivot;
[SerializeField] private GameObject turningFront;
[SerializeField] private GameObject turningBack;
[SerializeField] private float turnDuration = 0.6f;

private bool _isAnimating = false;
public bool IsAnimating => _isAnimating;
```

### `SetPageContent()` — target-agnostic content setter

Used for both static and turning pages. Replaces the inline content-setting lines that were previously in `UpdatePage`.

```csharp
private void SetPageContent(GameObject target, PageData data)
{
    if (target == null || data == null) return;
    target.GetComponentInChildren<TextMeshPro>().text = data.PageText;
    target.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", data.PageImage);
}
```

### `PlayPageTurn()` — DOTween Sequence

```csharp
private void PlayPageTurn(PageData nextLeft, PageData nextRight, int nextSpreadStart)
{
    _isAnimating = true;

    PageData currentRight = allPages.Find(p => !p.IsLeftPage && p.PageID == _currentSpreadStart + 1);

    SetPageContent(turningFront, currentRight); // page leaving
    SetPageContent(turningBack, nextLeft);       // page arriving

    // Pre-load static RightPage — hidden under TurningFront during first half
    SetPageContent(rightPage, nextRight);
    if (nextRight != null) rightPage.GetComponent<Page>().pageData = nextRight;

    turningPagePivot.transform.localEulerAngles = Vector3.zero;
    turningPagePivot.SetActive(true);

    Sequence seq = DOTween.Sequence();

    seq.Append(
        turningPagePivot.transform
            .DOLocalRotate(new Vector3(0f, -90f, 0f), turnDuration * 0.5f, RotateMode.Fast)
            .SetEase(Ease.InSine)
    );

    seq.AppendCallback(() =>
    {
        SetPageContent(leftPage, nextLeft);
        if (nextLeft != null) leftPage.GetComponent<Page>().pageData = nextLeft;
    });

    seq.Append(
        turningPagePivot.transform
            .DOLocalRotate(new Vector3(0f, -180f, 0f), turnDuration * 0.5f, RotateMode.Fast)
            .SetEase(Ease.OutSine)
    );

    seq.OnComplete(() =>
    {
        turningPagePivot.SetActive(false);
        turningPagePivot.transform.localEulerAngles = Vector3.zero;
        _currentSpreadStart = nextSpreadStart;
        _isAnimating = false;
    });
}
```

**Easing:** `InSine` on the first half (page starts slow, feels weighted). `OutSine` on the second half (decelerates as it lands). The split into two `Append` calls gives an exact frame-boundary hook for the `AppendCallback`.

### `NextPages()` — animation guard

```csharp
public void NextPages()
{
    if (_isAnimating) return;
    // ... rest of logic
}
```

`_isAnimating` is set to `true` at the start of `PlayPageTurn` and back to `false` in `OnComplete`. `Page.OnMouseDown` also checks `BookManager.Instance.IsAnimating` before calling `NextPages`.

---

## Inspector Setup

On the BookManager GameObject:

| Field | Assign |
|-------|--------|
| `Turning Page Pivot` | TurningPagePivot GO |
| `Turning Front` | TurningFront GO |
| `Turning Back` | TurningBack GO |
| `Turn Duration` | 0.6 (adjust to taste) |

---

## Rules

- TurningPagePivot must be **inactive in the prefab**. BookManager activates it at animation start and deactivates on complete.
- TurningFront and TurningBack must **not have MeshColliders** — clicks during animation are blocked at `Page.OnMouseDown`, not via colliders.
- Each turning quad needs its **own material instance** (`TurningFrontPage.mat`, `TurningBackPage.mat`). Do not reuse `BookPage.mat` directly.
- Do not call `NextPages()` from anywhere other than `Page.OnMouseDown` — the animation guard assumes a single call site.
- `turnDuration` is the total animation time. Each half takes `turnDuration * 0.5f` seconds.

---

← [Back to Overview](overview.md)
