using UnityEngine;
using Proyecto3.Environment;
using Proyecto3.Managers.SoundManager;

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
        SoundFXManager.instance.PlayRandomSoundFXClip(pickupSounds, gameObject.transform, 1f);

        // disable object so it cannot be grabbed again
        gameObject.SetActive(false);
    }
    public override void OnLoseFocus()
    {
        print("STOPPED LOOKING AT " + gameObject.name);

    }
}
