# Menu System

## Context

The game has three menus: a main menu (shown on the Menu scene), a pause menu, and a settings menu. All three are managed by `MenuManager`. UI element animations (scale on hover/select) and gamepad/keyboard navigation are handled by `MenuEventSystemHandler`.

---

## `MenuManager` — `Scripts/Managers/MenuManager.cs`

Controls menu visibility, scene loading, cursor state, and time scale.

### Inspector Fields

| Field | Description |
|-------|-------------|
| `mainMenu` | Main menu panel GameObject |
| `pauseMenu` | Pause menu panel GameObject |
| `settingsMenu` | Settings menu panel GameObject |

### Behaviour

On `Start()`: pause and settings menus are hidden, cursor state is updated to match.

On `Update()`: listens for `Escape` and calls `TogglePauseMenu()`.

### Methods

| Method | Description |
|--------|-------------|
| `StartGame()` | Hides main menu, loads next scene (build index + 1) |
| `TogglePauseMenu()` | Shows/hides pause menu, pauses/resumes time, hides settings if re-opening pause |
| `ToggleSettingsMenu()` | Shows/hides settings menu, also toggles pause menu |
| `ResumeGame()` | Hides pause menu, restores `Time.timeScale = 1` |
| `QuitGame()` | Calls `Application.Quit()` (and `EditorApplication.isPlaying = false` in editor) |
| `RestartLevel()` | Reloads the current scene |
| `LoadMainMenu()` | Loads scene at index 0 |

### Cursor State

`UpdateCursorState()` is called after any menu visibility change. The cursor is **visible and unlocked** whenever any menu is open, and **hidden and locked** when all menus are closed. `FirstPersonController` also gates movement on `!Cursor.visible`, so opening a menu automatically stops the player from moving.

---

## `MenuEventSystemHandler` — `Scripts/Managers/MenuEventSystemHandler.cs`

Handles UI navigation animations and sound for a set of `Selectable` elements (buttons, sliders, etc.). Uses DOTween for scale animations and Unity's `EventSystem` for selection tracking.

### Inspector Fields

| Field | Description |
|-------|-------------|
| `menuSelectables` | All `Selectable` UI elements in this menu |
| `_firstSelected` | The element focused when the menu opens |
| `_navigateReference` | InputActionReference for navigate (gamepad d-pad / keyboard arrows) |
| `_selectedAnimationScale` | Scale multiplier on select (e.g. `1.1`) |
| `_scaleDuration` | Duration of scale tween in seconds |
| `_animationExclusions` | GameObjects that skip the scale animation |
| `SoundEvent` | UnityEvent invoked when a selectable is selected (wire to a sound method) |

### Behaviour

On `OnEnable()`: resets all selectable scales to their original values, subscribes to the navigate input action, and selects `_firstSelected` after one frame (to ensure the EventSystem is ready).

On `OnDisable()`: unsubscribes from input and kills any active DOTween tweens.

**Select / Deselect** — scales the element up on select, back to original on deselect. Elements in `_animationExclusions` skip this.

**Pointer enter / exit** — routes mouse hover through the EventSystem so hover and keyboard navigation share the same select/deselect logic.

**Navigate** — if the EventSystem loses its selection (can happen on mouse click), re-selects the last focused element when navigating with a gamepad or keyboard.

### Extending

`MenuEventSystemHandler` is designed to be subclassed. All key methods are `virtual`. Override `OnEnable`, `OnDisable`, or `AddSelectionListeners` to customize behaviour for specific menus.

---

## Scene Flow

```
Menu scene (index 0)
  └── MenuManager.StartGame()
        └── SceneManager.LoadScene(1) → Main Scene
              └── (Escape) → MenuManager.TogglePauseMenu()
                    ├── Resume → MenuManager.ResumeGame()
                    ├── Settings → MenuManager.ToggleSettingsMenu()
                    ├── Restart → MenuManager.RestartLevel()
                    └── Main Menu → MenuManager.LoadMainMenu() → Menu scene
```

---

## Rules

- Wire all menu buttons to `MenuManager` methods via the inspector — do not call them from other scripts.
- `_firstSelected` must be assigned on every `MenuEventSystemHandler` instance, or the menu will have no default selection on open.
- All `Selectable` elements that should animate must be added to `menuSelectables`. Elements not in that list will not have their scale tracked and may break on deselect.
- Do not set `Time.timeScale = 0` anywhere outside `MenuManager` — it is exclusively managed there.

---

← [Back to Overview](overview.md)
