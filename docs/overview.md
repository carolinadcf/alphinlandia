# Alphinlandia — Project Overview

## What Is This

Alphinlandia is a first-person exploration game built in Unity. The player moves through an environment, interacts with objects, and can read in-game books. It is developed under the namespace prefix `Proyecto3`.

---

## Scenes

| Scene | Purpose |
|-------|---------|
| `Menu.unity` | Main menu with UI navigation, play/quit/settings |
| `Main Scene.unity` | Core gameplay scene with the player, environment, and book |

Scene order in Build Settings: Menu (index 0) → Main Scene (index 1).

---

## Tech Stack

| Tool | Usage |
|------|-------|
| Unity (URP) | Engine + render pipeline |
| TextMesh Pro | All in-game text rendering |
| DOTween (Demigiant) | UI animations and tweening |
| Unity Input System | Menu navigation input |
| Unity Audio Mixer | Volume control (master, SFX, music channels) |

---

## Project Structure

```
Assets/
├── Audio/                    Audio clips
├── Book - SO/                BookData + PageData ScriptableObject assets, book1.json
├── Materials/                Materials
├── Prefabs/
│   ├── Book.prefab           The in-game open book (left + right page GameObjects)
│   └── SoundFXObject.prefab  Pooled audio source for one-shot SFX
├── Resources/                Assets loaded via Resources.Load (DOTween settings)
├── Scenes/
│   ├── Menu.unity
│   └── Main Scene.unity
├── Scripts/
│   ├── Book/                 Book, BookData, Page, PageData
│   ├── Environment/          Interactable (abstract), TestInteractable
│   ├── Managers/             MenuManager, MenuEventSystemHandler, SoundFXManager, SoundMixerManager
│   └── Player/               FirstPersonController
├── Sprites/                  Textures and illustrations
└── BookManager.cs            BookManager (lives at Assets root, not in Scripts/)
```

---

## Namespaces

| Namespace | Contents |
|-----------|---------|
| `Proyecto3.Book` | `Book`, `BookData`, `BookManager`, `Page`, `PageData` |
| `Proyecto3.Player` | `FirstPersonController` |
| `Proyecto3.Environment` | `Interactable`, `TestInteractable` |
| `Proyecto3.Managers.MenuManager` | `MenuManager`, `MenuEventSystemHandler` |
| `Proyecto3.Managers.SoundManager` | `SoundFXManager`, `SoundMixerManager` |

---

## Architecture Patterns

**Singleton** — `BookManager` and `SoundFXManager` both expose a static `Instance`. Do not add multiple copies of these to a scene.

**Abstract base class** — `Interactable` defines the contract for all interactable objects (`OnInteract`, `OnFocus`, `OnLoseFocus`). Add new interactables by extending it.

**ScriptableObjects** — `BookData` and `PageData` are ScriptableObject types. They can be created as editor assets, but at runtime they are instantiated dynamically from JSON instead.

**JSON-driven content** — Book content is authored in `book1.json` and loaded at runtime by `BookManager`. No editor assets need to change to update book text or images.

---

## Key Relationships

```
FirstPersonController
  └── raycast → Interactable (layer 6)
        └── OnInteract() → implementation-defined behavior

BookManager (singleton)
  ├── reads book1.json → creates PageData instances
  ├── owns allPages list
  ├── drives left/right Page GameObjects in Book.prefab
  └── PlayPageTurn() → DOTween Sequence on TurningPagePivot

Page (MonoBehaviour on static page GameObjects)
  └── OnMouseDown() → IsAnimating check → BookManager.Instance.NextPages()

SoundFXManager (singleton)
  └── spawns SoundFXObject.prefab → plays clip → destroys self

MenuManager
  └── controls pause/settings/main menu visibility + cursor state
```

---

## Interactable Layer

All interactable objects must be on **layer 6** (`Interactable`). `Interactable.Awake()` sets this automatically. `FirstPersonController` raycasts only against this layer.

---

## Further Reading

| Document | Covers | Relevant to |
|----------|--------|-------------|
| [Book System](book-system.md) | `Book`, `BookData`, `Page`, `PageData`, `BookManager` — components, page turn flow, inspector setup, rules | The `Proyecto3.Book` namespace; the `Book.prefab` and `Book - SO/` assets listed in the project structure |
| [Page-Turn Animation](page-turn-animation.md) | DOTween page-turn sequence, TurningPagePivot hierarchy, animation phases, `PlayPageTurn`, `IsAnimating` guard | The `Book.prefab` turning page GameObjects; `PlayPageTurn` and `SetPageContent` in `BookManager`; DOTween in the tech stack |
| [Prefab Setup Guide](prefab-setup-guide.md) | Step-by-step Unity Editor instructions to set up the turning page GameObjects and wire BookManager inspector fields | Required one-time setup for the page-turn animation to work |
| [JSON Book Loading](json-book-loading.md) | How `BookManager` deserializes `book1.json` at runtime, texture loading, JSON contract | The `book1.json` file in `Book - SO/`; the `LoadBookFromJson` / `LoadTexture` methods in `BookManager` |
| [Player & Interaction](player-interaction.md) | `FirstPersonController`, `Interactable` abstract class, how to add new interactables | The `Proyecto3.Player` and `Proyecto3.Environment` namespaces; the Interactable layer (layer 6) noted in Key Relationships |
| [Menu System](menu-system.md) | `MenuManager`, `MenuEventSystemHandler`, cursor/time scale logic, scene flow | The `Proyecto3.Managers.MenuManager` namespace; the `Menu.unity` and `Main Scene.unity` scenes; how cursor state gates player movement |
| [Audio](audio.md) | `SoundFXManager`, `SoundMixerManager`, `SoundFXObject.prefab`, AudioMixer parameter names | The `Proyecto3.Managers.SoundManager` namespace; the `SoundFXObject.prefab` in `Prefabs/`; the `Audio/` folder |
