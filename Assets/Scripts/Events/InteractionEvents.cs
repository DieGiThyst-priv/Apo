using System;
using UnityEngine;

public class InteractionEvents
{
    public event Action<Interactable> OnInteract;

    public void Interact(Interactable interactable)
    {
        Debug.Log("Interacted with: " + interactable.name);
        if (OnInteract != null)
            OnInteract(interactable);
    }
}
