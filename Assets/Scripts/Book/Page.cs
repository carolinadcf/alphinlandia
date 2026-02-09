using UnityEngine;
using Proyecto3.Book;

public class Page : MonoBehaviour
{
    [SerializeField] PageData pageData;
    // right or left page
    [SerializeField] private bool isLeftPage;

    // Start is called before the first frame update
    void Start()
    {
        BookManager.Instance.UpdatePage(pageData, isLeftPage); // Update the book with this page's data (left page) and no right page
    }

    private void OnMouseDown()
    {
        // Example of how to handle page click
        Debug.Log("Page clicked!");
        Debug.Log(pageData.PageText);
    }
}
