# Book System

## Context

The book system renders an open book as a spread — two pages visible at once, a left and a right. The player clicks a page to advance to the next spread with an animated page turn. Content (text, images) is loaded at runtime from a JSON file so it can be edited without touching the Unity editor.

See [JSON Book Loading](json-book-loading.md) for the JSON pipeline details.
See [Page-Turn Animation](page-turn-animation.md) for the animation system details.

---

## Components

### `PageData` — `Scripts/Book/PageData.cs`

A `ScriptableObject` that holds the content of a single page.

| Field | Type | Description |
|-------|------|-------------|
| `_pageID` | `int` | Unique sequential ID. Even = left page, odd = right page. |
| `_pageText` | `string` | Text displayed on the page. |
| `_pageImage` | `Texture` | Image rendered on the page mesh. |
| `_isLeftPage` | `bool` | Whether this page sits on the left or right side. |
| `_nextPageID` | `int` | ID of the next page in sequence (used to store order in JSON). |

All fields are private with public read-only getters. At runtime, instances are created via `ScriptableObject.CreateInstance<PageData>()` and populated through `Initialize()`.

```csharp
public void Initialize(int id, string text, Texture image, bool isLeft, int nextId)
```

### `BookData` — `Scripts/Book/BookData.cs`

A `ScriptableObject` that groups a full book: title, author, and an array of `PageData`. This is the legacy editor-asset approach. At runtime, `BookManager` bypasses `BookData` entirely and builds pages from JSON.

### `Book` — `Scripts/Book/Book.cs`

A thin `MonoBehaviour` on the book GameObject that holds a reference to a `BookData` asset. Currently used as a holder; the active runtime logic lives in `BookManager`.

### `Page` — `Scripts/Book/Page.cs`

Attached to the static page GameObjects inside `Book.prefab` (LeftPage and RightPage). Holds a reference to its current `PageData`.

```csharp
public PageData pageData;
```

When the player clicks a page, `OnMouseDown()` fires. It checks `BookManager.Instance.IsAnimating` first — if a turn is already in progress, the click is ignored. Otherwise it calls `NextPages()`.

### `BookManager` — `BookManager.cs`

The central controller. Singleton. Lives at `Assets/BookManager.cs` (not inside `Scripts/`).

**Responsibilities:**
- Deserializes `book1.json` into a runtime list of `PageData`
- Loads textures referenced in the JSON
- Tracks the current spread position (`_currentSpreadStart`)
- Updates the left and right `Page` GameObjects when the spread changes

**Key methods:**

| Method | Description |
|--------|-------------|
| `LoadBookFromJson(string json)` | Parses JSON, creates PageData instances, shows first spread |
| `ShowSpread(int leftPageID)` | Finds left and right PageData for a given spread and calls `UpdatePage` on both |
| `UpdatePage(PageData pageData)` | Routes to the correct static page GO and calls `SetPageContent` |
| `SetPageContent(GameObject, PageData)` | Sets text and texture on any target GO — used for static and turning pages |
| `NextPages()` | Guards against animation, then calls `PlayPageTurn()` |
| `PlayPageTurn(...)` | Runs the DOTween Sequence for the page-turn animation |
| `LoadTexture(string path)` | Loads a texture from a path (editor: `AssetDatabase`, runtime: `Resources.Load`) |

`IsAnimating` (public bool property) is exposed so `Page.OnMouseDown` can block clicks mid-animation.

---

## Page Turn Flow

```
Player clicks page
  └── Page.OnMouseDown()
        ├── IsAnimating? → ignore click
        └── BookManager.Instance.NextPages()
              └── PlayPageTurn(nextLeft, nextRight, nextSpreadStart)
                    ├── load TurningFront with current right content
                    ├── load TurningBack with next left content
                    ├── pre-load static RightPage with next right content
                    ├── DOTween: pivot 0° → -90° (InSine)
                    ├── AppendCallback: swap static LeftPage content (edge-on, invisible)
                    ├── DOTween: pivot -90° → -180° (OutSine)
                    └── OnComplete: deactivate pivot, advance _currentSpreadStart
```

Both pages always update together as a single spread. The animation prevents any out-of-sync state via the `_isAnimating` guard.

---

## Page ID Convention

Pages are numbered sequentially starting from 0:

| PageID | Side |
|--------|------|
| 0 | Left |
| 1 | Right |
| 2 | Left |
| 3 | Right |
| … | … |

`_currentSpreadStart` always holds an even number (the left page ID of the current spread). `NextPages()` adds 2 each time.

---

## Inspector Setup (BookManager GameObject)

| Field | What to assign |
|-------|----------------|
| `Left Page` | LeftPage GO inside Book.prefab |
| `Right Page` | RightPage GO inside Book.prefab |
| `All Pages` | Leave empty — populated at runtime from JSON |
| `Book Json` | Drag `book1.json` here |
| `Turning Page Pivot` | TurningPagePivot GO inside Book.prefab |
| `Turning Front` | TurningFront GO inside Book.prefab |
| `Turning Back` | TurningBack GO inside Book.prefab |
| `Turn Duration` | Animation length in seconds (default 0.6) |

---

## Adding a New Book

1. Duplicate `book1.json` and fill in new content.
2. Place any images under `Assets/Sprites/` (editor) or `Assets/Resources/` (builds).
3. Assign the new JSON file to the `Book Json` field on the BookManager GameObject.
4. Keep PageIDs sequential starting from 0, even IDs left, odd IDs right.

---

## Rules

- `BookManager` is a singleton — only one instance per scene.
- Do not manually populate `allPages` in the inspector; it is cleared and rebuilt from JSON on `Start()`.
- Page GameObjects must have a `Page` component and a `TextMeshPro` + `Renderer` somewhere in their children.
- The `_BaseMap` shader property is used to set the page image on the material.

---

← [Back to Overview](overview.md)
