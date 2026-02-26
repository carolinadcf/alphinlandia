# Player & Interaction System

## Context

The player is a first-person character built on Unity's `CharacterController`. They can look around, walk, and interact with objects in the world. Interaction is raycast-based: the player looks at an object and clicks to trigger it.

Movement is currently **disabled by default** (`canMove = false` in the inspector). This is intentional while the book/reading feature is being developed.

---

## `FirstPersonController` — `Scripts/Player/FirstPersonController.cs`

Single MonoBehaviour on the Player GameObject. Requires a `CharacterController` component and a child `Camera`.

### Inspector Fields

| Field | Default | Description |
|-------|---------|-------------|
| `canInteract` | `true` | Enables interaction raycasting |
| `canMove` | `false` | Enables movement and mouse look |
| `interactKey` | `Mouse0` | Key that triggers `OnInteract()` |
| `walkSpeed` | `3.0` | Movement speed |
| `gravity` | `30.0` | Gravity applied when airborne |
| `lookSpeedX/Y` | `2.0` | Mouse sensitivity |
| `upperLookLimit` | `80°` | Max upward camera angle |
| `lowerLookLimit` | `80°` | Max downward camera angle |
| `interactionRayPoint` | `Vector3` | Viewport point for the interaction raycast (center = `0.5, 0.5, 0`) |
| `interactionDistance` | `float` | Max distance to detect interactables |
| `interactionLayer` | `LayerMask` | Should include layer 6 (Interactable) |

### Behaviour

Every `Update()`, if `canMove` is true and the cursor is hidden (i.e. no menu is open):
1. **Movement** — WASD/arrow keys via `Input.GetAxis`, applied through `CharacterController.Move`
2. **Mouse look** — vertical rotation clamped between `upperLookLimit` and `lowerLookLimit`
3. **Interaction check** — raycast from the camera viewport point; if it hits layer 6, calls `OnFocus()` on the interactable. When the ray stops hitting, calls `OnLoseFocus()`
4. **Interaction input** — on `interactKey` down, if an interactable is in range, calls `OnInteract()`

The cursor is expected to be locked (`CursorLockMode.Locked`) during gameplay. Movement and interaction are gated on `!Cursor.visible`, so opening any menu automatically disables them.

---

## `Interactable` — `Scripts/Environment/Interactable.cs`

Abstract base class for all interactable objects in the world.

```csharp
public abstract class Interactable : MonoBehaviour
{
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
```

`Awake()` automatically assigns the GameObject to **layer 6** (`Interactable`). Any subclass that overrides `Awake()` must call `base.Awake()` to preserve this.

### Adding a New Interactable

1. Create a new script that extends `Interactable`.
2. Implement the three abstract methods.
3. Attach it to a GameObject in the scene — layer 6 is set automatically.
4. Make sure `interactionLayer` on `FirstPersonController` includes layer 6.

---

## `TestInteractable` — `Scripts/Environment/TestInteractable.cs`

A reference implementation of `Interactable`. On interact: logs to console, plays a random pickup sound, and disables the GameObject so it cannot be picked up again.

```csharp
public override void OnInteract()
{
    SoundFXManager.instance.PlayRandomSoundFXClip(pickupSounds, transform, 1f);
    gameObject.SetActive(false);
}
```

Use this as a template when building real interactables.

---

## Interaction Flow

```
Update()
  └── HandleInteractionCheck()
        └── Raycast from camera viewport
              ├── Hit layer 6 → currentInteractable.OnFocus()
              └── No hit (was focused) → currentInteractable.OnLoseFocus()

Update()
  └── HandleInteractionInput()
        └── interactKey pressed + hit in range
              └── currentInteractable.OnInteract()
```

---

## Rules

- Every interactable **must** be on layer 6. This happens automatically via `Interactable.Awake()`.
- Do not implement interaction logic directly in `FirstPersonController` — extend `Interactable` instead.
- The interaction raycast uses a **viewport point**, not a world position. Set `interactionRayPoint` to `(0.5, 0.5, 0)` for a center-screen crosshair.
- Movement is intentionally disabled (`canMove = false`) while book interaction is the primary mechanic. Re-enable it in the inspector when building out world exploration.

---

← [Back to Overview](overview.md)
