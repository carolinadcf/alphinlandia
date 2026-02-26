# Book System

## Context

The book system renders an open book as a spread — two pages visible at once, a left and a right. The player clicks a page to advance to the next spread. Content (text, images) is loaded at runtime from a JSON file so it can be edited without touching the Unity editor.

See [JSON Book Loading](json-book-loading.md) for full implementation details of the JSON pipeline.

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

Attached to each page GameObject inside `Book.prefab` (one for left, one for right). Holds a reference to its current `PageData`.

```csharp
public PageData pageData;
```

When the player clicks a page, `OnMouseDown()` fires and calls `BookManager.Instance.NextPages()`. This is the only input trigger for page turning.

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
| `UpdatePage(PageData pageData)` | Sets text and texture on the appropriate page GameObject |
| `NextPages()` | Advances `_currentSpreadStart` by 2 and calls `ShowSpread` |
| `LoadTexture(string path)` | Loads a texture from a path (editor: `AssetDatabase`, runtime: `Resources.Load`) |

---

## Page Turn Flow

```
Player clicks page
  └── Page.OnMouseDown()
        └── BookManager.Instance.NextPages()
              └── _currentSpreadStart += 2
                    └── ShowSpread(_currentSpreadStart)
                          ├── find PageData where IsLeftPage && PageID == leftID  → UpdatePage()
                          └── find PageData where !IsLeftPage && PageID == leftID+1 → UpdatePage()
```

Both pages always update together as a single spread. It is not possible for left and right to get out of sync.

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
| `Left Page` | The left page GameObject inside Book.prefab |
| `Right Page` | The right page GameObject inside Book.prefab |
| `All Pages` | Leave empty — populated at runtime from JSON |
| `Book Json` | Drag `book1.json` here |

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
