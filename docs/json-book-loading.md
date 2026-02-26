# JSON Book Loading

## Context

Alphinlandia is a game that includes an in-game book the player can read. The book is displayed as an open spread — two pages visible at a time, a left and a right — and the player can turn pages forward.

The book UI is built around three key components:

- **`PageData`** — a ScriptableObject that holds the content of a single page: an ID, text, an image texture, which side it appears on (left/right), and the ID of the next page.
- **`BookManager`** — a MonoBehaviour singleton that owns the list of all pages, controls which spread is currently shown, and handles page-turn logic.
- **`Page`** — a MonoBehaviour attached to each page GameObject (left and right) in the scene. It holds a reference to its current `PageData` and is updated by `BookManager` when the spread changes.

Originally, all page content was defined manually in the Unity editor via ScriptableObject assets. The JSON loading system replaces that workflow: a single `book1.json` file describes the entire book, and `BookManager` reads it at runtime to build the page list dynamically. This makes it possible to edit book content without ever opening the Unity editor.

---

## Summary

The book system previously relied on ScriptableObject assets (BookData, PageData) manually configured in the Unity editor. This change adds runtime JSON loading to BookManager so book content can be authored in plain JSON without touching Unity editor assets. Page navigation was also reworked to advance both pages together as a synchronized spread.

---

## Files Modified

| File | Change |
|------|--------|
| `Assets/Scripts/Book/PageData.cs` | Added `Initialize()` method |
| `Assets/BookManager.cs` | Added JSON DTOs, `_bookJson` field, `Start()`, `LoadBookFromJson()`, `LoadTexture()`, spread-based navigation |

---

## Implementation

### `PageData.cs` — `Initialize()`

`PageData` fields are private with read-only getters. Added one public method to set all fields after `ScriptableObject.CreateInstance<PageData>()`:

```csharp
public void Initialize(int id, string text, Texture image, bool isLeft, int nextId)
{
    _pageID     = id;
    _pageText   = text;
    _pageImage  = image;
    _isLeftPage = isLeft;
    _nextPageID = nextId;
}
```

### `BookManager.cs` — JSON DTOs

Two serializable DTOs nested inside `BookManager`:

```csharp
[System.Serializable]
private class PageJsonData
{
    public int PageID;
    public string PageText;
    public string PageImage;   // e.g. "Assets/Sprites/Example Page Illustration.jpg"
    public bool IsLeftPage;
    public int NextPageID;
}

[System.Serializable]
private class BookJsonData
{
    public string BookTitle;
    public string Author;
    public PageJsonData[] Pages;
}
```

### `BookManager.cs` — Fields

```csharp
[SerializeField] private TextAsset _bookJson;
private int _currentSpreadStart = 0;
```

`_currentSpreadStart` tracks the PageID of the left page of the currently displayed spread.

### `BookManager.cs` — `Start()`

```csharp
private void Start()
{
    if (_bookJson != null)
        LoadBookFromJson(_bookJson.text);
}
```

### `BookManager.cs` — `LoadBookFromJson()`

Deserializes the JSON, creates `PageData` instances at runtime, and displays the first spread:

```csharp
private void LoadBookFromJson(string json)
{
    BookJsonData bookData = JsonUtility.FromJson<BookJsonData>(json);
    if (bookData == null || bookData.Pages == null) return;

    allPages.Clear();

    foreach (PageJsonData p in bookData.Pages)
    {
        PageData page = ScriptableObject.CreateInstance<PageData>();
        Texture texture = LoadTexture(p.PageImage);
        page.Initialize(p.PageID, p.PageText, texture, p.IsLeftPage, p.NextPageID);
        allPages.Add(page);
    }

    _currentSpreadStart = 0;
    ShowSpread(_currentSpreadStart);
}
```

### `BookManager.cs` — `LoadTexture()`

In the editor, uses `AssetDatabase.LoadAssetAtPath` so images can be loaded from anywhere in the project (e.g. `Assets/Sprites/`). Falls back to `Resources.Load` for runtime builds (images must be under `Assets/Resources/` in that case).

```csharp
private Texture LoadTexture(string path)
{
#if UNITY_EDITOR
    Texture editorTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(path);
    if (editorTexture != null) return editorTexture;
#endif
    const string prefix = "Assets/Resources/";
    if (path.StartsWith(prefix))
        path = path.Substring(prefix.Length);

    int dot = path.LastIndexOf('.');
    if (dot >= 0)
        path = path.Substring(0, dot);

    return Resources.Load<Texture>(path);
}
```

### `BookManager.cs` — Spread Navigation

Pages advance as a synchronized spread. `NextPages()` advances `_currentSpreadStart` by 2 and calls `ShowSpread()`, which always updates both pages together.

```csharp
public void NextPages()
{
    int nextSpreadStart = _currentSpreadStart + 2;
    if (allPages.Find(p => p.PageID == nextSpreadStart) == null)
    {
        Debug.Log("End of book.");
        return;
    }
    _currentSpreadStart = nextSpreadStart;
    ShowSpread(_currentSpreadStart);
}

private void ShowSpread(int leftPageID)
{
    PageData leftPageData  = allPages.Find(p => p.IsLeftPage  && p.PageID == leftPageID);
    PageData rightPageData = allPages.Find(p => !p.IsLeftPage && p.PageID == leftPageID + 1);

    if (leftPageData  != null) UpdatePage(leftPageData);
    if (rightPageData != null) UpdatePage(rightPageData);
}
```

---

## JSON Contract

Pages must be sequential: even IDs are left pages, odd IDs are right pages. Images can be referenced from anywhere under `Assets/` when running in the editor. For runtime builds, images must be under `Assets/Resources/`.

```json
{
  "BookTitle": "Una Tiefling Salvaje",
  "Author": "Pijamada Real",
  "Pages": [
    { "PageID": 0, "PageText": "...", "PageImage": "Assets/Sprites/Image.jpg", "IsLeftPage": true,  "NextPageID": 1 },
    { "PageID": 1, "PageText": "...", "PageImage": "Assets/Sprites/Image.jpg", "IsLeftPage": false, "NextPageID": 2 }
  ]
}
```

---

## Inspector Setup

1. Drag `book1.json` into the `Book Json` field on the BookManager GameObject.
2. Image files can be anywhere under `Assets/` for editor testing. For builds, place them under `Assets/Resources/`.

---

## Verification

1. Enter Play Mode — both pages should display text and images from the JSON.
2. Click next — both pages should update together as a single spread.
3. Reach the last spread — console should log "End of book." with no errors.
4. Changing the JSON content (title, text, image paths) should reflect without touching ScriptableObject assets.

---

← [Back to Overview](overview.md)
