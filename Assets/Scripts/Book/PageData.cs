using UnityEngine;

namespace Proyecto3.Book
{
    [CreateAssetMenu(fileName = "New PageData", menuName = "Scriptable Objects/PageData")]
    public class PageData : ScriptableObject
    {
        [SerializeField] private Texture _pageImage;
        [SerializeField] private string _pageText;

        // getters
        public Texture PageImage { get { return _pageImage; } }
        public string PageText { get { return _pageText; } }
    }

}