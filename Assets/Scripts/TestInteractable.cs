using UnityEngine;

public class TestInteractable : Interactable
{
    public override void OnFocus()
    {
        print("LOOKING AT " + gameObject.name);
    }
    public override void OnInteract()
    {
        print("INTERACTED WITH " + gameObject.name);

    }
    public override void OnLoseFocus()
    {
        print("STOPPED LOOKING AT " + gameObject.name);

    }
}
