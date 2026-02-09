using UnityEngine;

namespace Proyecto3.Book
{
    public class Book : MonoBehaviour
    {
        [SerializeField] private BookData bookData;

        public BookData BookData { get { return bookData; } set { bookData = value; } }
    }
}