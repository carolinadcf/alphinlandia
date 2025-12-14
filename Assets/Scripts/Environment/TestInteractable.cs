using UnityEngine;

public class TestInteractable : Interactable
{
    public string itemName = "Test Item";
    public int itemID = 1;
    public Sprite itemIcon;

    [SerializeField] private AudioClip[] pickupSounds;

    public override void OnFocus()
    {
        print("LOOKING AT " + gameObject.name);
    }
    public override void OnInteract()
    {
        print("INTERACTED WITH " + gameObject.name);
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(new InventoryItem(itemName, itemID, itemIcon));
            SoundFXManager.instance.PlayRandomSoundFXClip(pickupSounds, gameObject.transform, 1f);
        }

        // disable object so it cannot be grabbed again
        gameObject.SetActive(false);
    }
    public override void OnLoseFocus()
    {
        print("STOPPED LOOKING AT " + gameObject.name);

    }
}
