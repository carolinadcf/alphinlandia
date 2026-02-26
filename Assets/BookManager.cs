using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

namespace Proyecto3.Book
{
    public class BookManager : MonoBehaviour
    {
        [SerializeField] private GameObject leftPage;
        [SerializeField] private GameObject rightPage;
        [SerializeField] public List<PageData> allPages; // List of all page data scriptable objects
        [SerializeField] private TextAsset _bookJson;
        private int _currentSpreadStart = 0;

        [System.Serializable]
        private class PageJsonData
        {
            public int PageID;
            public string PageText;
            public string PageImage;
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

        // Singleton instance
        public static BookManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (_bookJson != null)
                LoadBookFromJson(_bookJson.text);
        }

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

        public void UpdatePage(PageData pageData)
        {
            if (pageData != null)
            {
                // access page data and update the book prefab accordingly
                if (pageData.IsLeftPage)
                {
                    leftPage.GetComponent<Page>().pageData = pageData; // set the page data of the left page to the current page data

                    // update page content based on new page data
                    leftPage.GetComponentInChildren<TextMeshPro>().text = pageData.PageText;
                    // change base map texture of the left page material to pageData.PageImage
                    leftPage.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", pageData.PageImage);
                }
                else
                {
                    rightPage.GetComponent<Page>().pageData = pageData; // set the page data of the right page to the current page data

                    rightPage.GetComponentInChildren<TextMeshPro>().text = pageData.PageText;
                    rightPage.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", pageData.PageImage);
                }
            }
        }

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
    }
}