using UnityEngine;
using Proyecto3.Book;

public class Page : MonoBehaviour
{
    [SerializeField] public PageData pageData;

    // Start is called before the first frame update
    void Start()
    {
        // BookManager.Instance.UpdatePage(pageData);
    }

    private void OnMouseDown()
    {
        BookManager.Instance.NextPages();
    }
}
