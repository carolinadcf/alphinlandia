using UnityEngine;

namespace Proyecto3.Book
{
    [CreateAssetMenu(fileName = "New PageData", menuName = "Scriptable Objects/PageData")]
    public class PageData : ScriptableObject
    {
        [SerializeField] private int _pageID;
        [SerializeField] private Texture _pageImage;
        [SerializeField] private string _pageText;
        [SerializeField] private bool _isLeftPage;
        [SerializeField] private int _nextPageID;

        // getters
        public int PageID { get { return _pageID; } }
        public Texture PageImage { get { return _pageImage; } }
        public string PageText { get { return _pageText; } }
        public bool IsLeftPage { get { return _isLeftPage; } }
        public int NextPageID { get { return _nextPageID; } }
    }

}