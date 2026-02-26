using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

namespace Proyecto3.Book
{
    public class BookManager : MonoBehaviour
    {
        [SerializeField] private GameObject leftPage;
        [SerializeField] private GameObject rightPage;
        [SerializeField] public List<PageData> allPages;
        [SerializeField] private TextAsset _bookJson;

        [Header("Turning Page")]
        [SerializeField] private GameObject turningPagePivot;
        [SerializeField] private GameObject turningFront;
        [SerializeField] private GameObject turningBack;
        [SerializeField] private float turnDuration = 0.6f;

        private int _currentSpreadStart = 0;
        private bool _isAnimating = false;
        public bool IsAnimating => _isAnimating;

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

        private void SetPageContent(GameObject target, PageData data)
        {
            if (target == null || data == null) return;
            target.GetComponentInChildren<TextMeshPro>().text = data.PageText;
            target.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", data.PageImage);
        }

        public void UpdatePage(PageData pageData)
        {
            if (pageData == null) return;
            if (pageData.IsLeftPage)
            {
                leftPage.GetComponent<Page>().pageData = pageData;
                SetPageContent(leftPage, pageData);
            }
            else
            {
                rightPage.GetComponent<Page>().pageData = pageData;
                SetPageContent(rightPage, pageData);
            }
        }

        public void NextPages()
        {
            if (_isAnimating) return;

            int nextSpreadStart = _currentSpreadStart + 2;
            if (allPages.Find(p => p.PageID == nextSpreadStart) == null)
            {
                Debug.Log("End of book.");
                return;
            }

            PageData nextLeft  = allPages.Find(p =>  p.IsLeftPage && p.PageID == nextSpreadStart);
            PageData nextRight = allPages.Find(p => !p.IsLeftPage && p.PageID == nextSpreadStart + 1);

            PlayPageTurn(nextLeft, nextRight, nextSpreadStart);
        }

        private void PlayPageTurn(PageData nextLeft, PageData nextRight, int nextSpreadStart)
        {
            if (turningPagePivot == null || turningFront == null || turningBack == null)
            {
                Debug.LogWarning("BookManager: Turning page objects not assigned in inspector. Falling back to instant swap.");
                _currentSpreadStart = nextSpreadStart;
                ShowSpread(_currentSpreadStart);
                return;
            }

            _isAnimating = true;

            // Current right page = the page being "lifted" away
            PageData currentRight = allPages.Find(p => !p.IsLeftPage && p.PageID == _currentSpreadStart + 1);

            // Load turning page faces
            SetPageContent(turningFront, currentRight); // page leaving
            SetPageContent(turningBack, nextLeft);       // page arriving

            // Pre-load static right page with next-right content
            // (hidden under TurningFront during first half)
            SetPageContent(rightPage, nextRight);
            if (nextRight != null) rightPage.GetComponent<Page>().pageData = nextRight;

            // Activate and reset pivot
            turningPagePivot.transform.localEulerAngles = Vector3.zero;
            turningPagePivot.SetActive(true);

            Sequence seq = DOTween.Sequence();

            // First half: 0° → -90°, page lifts to vertical (InSine = starts slow, feels weighted)
            seq.Append(
                turningPagePivot.transform
                    .DOLocalRotate(new Vector3(0f, -90f, 0f), turnDuration * 0.5f, RotateMode.Fast)
                    .SetEase(Ease.InSine)
            );

            // Midpoint: page is edge-on and invisible — silently swap static LeftPage
            seq.AppendCallback(() =>
            {
                SetPageContent(leftPage, nextLeft);
                if (nextLeft != null) leftPage.GetComponent<Page>().pageData = nextLeft;
            });

            // Second half: -90° → -180°, page lands on the left (OutSine = decelerates, settles)
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

        private void ShowSpread(int leftPageID)
        {
            PageData leftPageData  = allPages.Find(p => p.IsLeftPage  && p.PageID == leftPageID);
            PageData rightPageData = allPages.Find(p => !p.IsLeftPage && p.PageID == leftPageID + 1);

            if (leftPageData  != null) UpdatePage(leftPageData);
            if (rightPageData != null) UpdatePage(rightPageData);
        }
    }
}
