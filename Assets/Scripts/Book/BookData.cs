using UnityEngine;

namespace Proyecto3.Book
{
    [CreateAssetMenu(fileName = "BookData", menuName = "Scriptable Objects/BookData")]
    public class BookData : ScriptableObject
    {
        [SerializeField] private string _bookTitle;
        [SerializeField] private string _author;
        [SerializeField] private PageData[] _pages;

        // getters
        public string BookTitle { get { return _bookTitle; } }
        public string Author { get { return _author; } }
        public PageData[] Pages { get { return _pages; } }
    }
}