# JSON Book Loading

## Summary

The book system previously relied on ScriptableObject assets (BookData, PageData) manually configured in the Unity editor. This change adds runtime JSON loading to BookManager so book content can be authored in plain JSON without touching Unity editor assets.

---

## Files Modified

| File | Change |
|------|--------|
| `Assets/Scripts/Book/PageData.cs` | Added `Initialize()` method |
| `Assets/BookManager.cs` | Added JSON DTOs, `_bookJson` field, `Start()`, `LoadBookFromJson()`, `LoadTexture()` |

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
    public string PageImage;   // e.g. "Assets/Resources/Example Page Illustration.jpg"
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

### `BookManager.cs` — Serialized Field

```csharp
[SerializeField] private TextAsset _bookJson;
```

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

    PageData firstLeft  = allPages.Find(p => p.IsLeftPage  && p.PageID == 0);
    PageData firstRight = allPages.Find(p => !p.IsLeftPage && p.PageID == 1);
    if (firstLeft  != null) UpdatePage(firstLeft);
    if (firstRight != null) UpdatePage(firstRight);
}
```

### `BookManager.cs` — `LoadTexture()`

Strips the `Assets/Resources/` prefix and file extension so `Resources.Load` works:

```csharp
private Texture LoadTexture(string path)
{
    const string prefix = "Assets/Resources/";
    if (path.StartsWith(prefix))
        path = path.Substring(prefix.Length);

    int dot = path.LastIndexOf('.');
    if (dot >= 0)
        path = path.Substring(0, dot);

    return Resources.Load<Texture>(path);
}
```

---

## JSON Contract

Images must live under `Assets/Resources/`. The path in JSON should be `"Assets/Resources/<filename>.<ext>"`.

```json
{
  "BookTitle": "Una Tiefling Salvaje",
  "Author": "Pijamada Real",
  "Pages": [
    { "PageID": 0, "PageText": "...", "PageImage": "Assets/Resources/Image.jpg", "IsLeftPage": true,  "NextPageID": 1 },
    { "PageID": 1, "PageText": "...", "PageImage": "Assets/Resources/Image.jpg", "IsLeftPage": false, "NextPageID": 2 }
  ]
}
```

---

## Inspector Setup

1. Drag `book1.json` into the `Book Json` field on the BookManager GameObject.
2. Make sure all image files referenced in the JSON are under `Assets/Resources/`.

---

## Verification

1. Enter Play Mode — both pages should display text and images from the JSON.
2. Click a page — `NextPages()` should advance correctly using the runtime `allPages` list.
3. Changing the JSON content (title, text, image paths) should reflect without touching ScriptableObject assets.
