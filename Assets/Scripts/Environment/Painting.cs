using UnityEngine;
using UnityEngine.UI;

public class Painting : Interactable
{
    public string itemName = "Painting";
    public int itemID = 2;
    public Sprite itemIcon;

    public Sprite paintingSprite;
    private Renderer paintingRenderer;

    public GameObject paintingPanel;
    private Image paintingImage;

    private void Start()
    {
        // Apply sprite to UI
        paintingImage = GetComponentInChildren<Image>(true); // true = search inactive children too
        if (paintingImage != null)
            paintingImage.sprite = paintingSprite;

        // Apply sprite texture to material on mesh
        paintingRenderer = GetComponent<Renderer>();
        if (paintingRenderer != null && paintingSprite != null)
        {
            paintingRenderer.material.mainTexture = paintingSprite.texture;
        }

        // Adjust panel and mesh size based on sprite
        AdjustPanelSize();
        AdjustMeshSize();
    }

    public override void OnFocus()
    {
        print("LOOKING AT " + gameObject.name);
    }
    public override void OnInteract()
    {
        print("INTERACTED WITH " + gameObject.name);

        paintingPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public override void OnLoseFocus()
    {
        print("STOPPED LOOKING AT " + gameObject.name);

    }

    public void OnClickCloseButton()
    {
        print("CLOSE BUTTON CLICKED");

        paintingPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void AdjustPanelSize()
    {
        if (paintingPanel != null && paintingImage != null && paintingSprite != null)
        {
            float aspectRatio = (float)paintingSprite.texture.width / paintingSprite.texture.height;

            // Get 70% of screen height in pixels
            float screenHeight = Screen.height;
            float fixedHeight = screenHeight * 0.7f;

            // Adjust width based on aspect ratio
            float adjustedWidth = fixedHeight * aspectRatio;

            // Set Image size
            paintingImage.rectTransform.sizeDelta = new Vector2(adjustedWidth, fixedHeight);
            // Set Panel size
            paintingPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(adjustedWidth + 20, fixedHeight + 20); // Add some padding
        }
    }
    private void AdjustMeshSize()
    {
        if (paintingRenderer != null && paintingSprite != null)
        {
            float aspectRatio = (float)paintingSprite.texture.width / paintingSprite.texture.height;

            // Keep Y scale = 1 (height), adjust X scale only
            float fixedHeight = 0.1f;
            float adjustedWidth = fixedHeight * aspectRatio;

            paintingRenderer.transform.localScale = new Vector3(adjustedWidth, 0.1f, 0.1f);
        }
    }

}
