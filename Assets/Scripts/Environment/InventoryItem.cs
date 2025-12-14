using UnityEngine;

public class InventoryItem
{
    public string itemName;
    public int itemID;
    public Sprite itemIcon;

    public InventoryItem(string name, int id, Sprite icon)
    {
        itemName = name;
        itemID = id;
        itemIcon = icon;
    }   
}
