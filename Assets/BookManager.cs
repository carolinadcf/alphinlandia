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
            // current page data - left page
            PageData leftPageData = leftPage.GetComponent<Page>().pageData;
            nextPage(leftPageData);

            // current page data - right page
            PageData rightPageData = rightPage.GetComponent<Page>().pageData;
            nextPage(rightPageData);
        }

        private void nextPage(PageData currentPageData)
        {
            int nextPageID = currentPageData.NextPageID;
            PageData nextPageData = allPages.Find(page => page.PageID == nextPageID);

            Debug.Log("Clicked on page with ID: " + currentPageData.PageID + ". Next page ID: " + nextPageID);
            if (nextPageData != null)
            {
                UpdatePage(nextPageData);
            }
            else
            {
                Debug.LogWarning("No page found with ID: " + nextPageID);
            }
        }
    }
}