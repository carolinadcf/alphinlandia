using TMPro;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI staminaText = default;
    // [SerializeField] private TextMeshProUGUI inventoryText = default;
    
    // add a container for the inventory UI slots
    [SerializeField] private Transform inventoryPanel = default;
    // prefab that contains an Image component for an inventory slot
    [SerializeField] private GameObject inventoryItemPrefab = default;

    private void OnEnable()
    {
        FirstPersonController.OnStaminaChange += UpdateStamina;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += UpdateInventory;
        }
    }

    private void OnDisable()
    {
        FirstPersonController.OnStaminaChange -= UpdateStamina;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= UpdateInventory;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateStamina(100);
        if (InventoryManager.Instance != null)
        {
            UpdateInventory(InventoryManager.Instance.GetItems());
        }
    }

    private void UpdateStamina(float currentStamina)
    {
        staminaText.text = currentStamina.ToString("00");
    }

    // Updates the inventory UI text with the item icons
    private void UpdateInventory(List<InventoryItem> items)
    {
        // clear existing UI slots
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }

        // instantiate a UI slot for each inventory item
        foreach (InventoryItem item in items)
        {
            if (item.itemIcon != null)
            {
                GameObject slot = Instantiate(inventoryItemPrefab, inventoryPanel);
                
                // set the icon on the Image component
                Image iconImage = slot.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = item.itemIcon;
                }
            }
        }
    }
}
