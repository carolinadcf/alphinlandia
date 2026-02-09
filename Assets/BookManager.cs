using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Proyecto3.Book
{
    public class BookManager : MonoBehaviour
    {
        [SerializeField] private GameObject leftPage;
        [SerializeField] private GameObject rightPage;

        // Singleton instance
        private static BookManager instance;
        public static BookManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<BookManager>();
                }
                return instance;
            }
        }

        public void UpdatePage(PageData pageData, bool isLeftPage)
        {
            if (pageData != null)
            {
                // access page data and update the book prefab accordingly
                // Debug.Log("Updating book with page data: " + pageData.PageText);
                if (isLeftPage)
                {
                    leftPage.GetComponentInChildren<TextMeshPro>().text = pageData.PageText;
                    // change base map texture of the left page material to pageData.PageImage
                    leftPage.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", pageData.PageImage);
                }
                else
                {
                    rightPage.GetComponentInChildren<TextMeshPro>().text = pageData.PageText;
                    rightPage.GetComponentInChildren<Renderer>().material.SetTexture("_BaseMap", pageData.PageImage);
                }
            }
        }
    }
}