using UnityEngine;

public class CompanionInteract : Interactable
{
    public override void Interact(GameObject interactor)
    {
        Debug.Log("Hi!" + this.name);
    }
}
