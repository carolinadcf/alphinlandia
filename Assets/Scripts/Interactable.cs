using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public virtual void Awake ()
    {
        gameObject.layer = 6; // Interactable layer
    }
    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
