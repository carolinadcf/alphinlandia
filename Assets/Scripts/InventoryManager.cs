using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }
    private List<InventoryItem> items = new List<InventoryItem>();
    
    // Maximum number of items allowed in the inventory
    [SerializeField] private int maxInventorySize = 16;

    // Event to notify when the inventory changes
    public event Action<List<InventoryItem>> OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(InventoryItem newItem)
    {
        if (items.Count >= maxInventorySize)
        {
            Debug.Log("Inventory full! Cannot add item: " + newItem.itemName);
            return;
        }
        items.Add(newItem);
        Debug.Log("Added item: " + newItem.itemName);
        
        // Notify any listeners that the inventory has changed
        OnInventoryChanged?.Invoke(items);
    }

    public List<InventoryItem> GetItems()
    {
        return items;
    }
}
