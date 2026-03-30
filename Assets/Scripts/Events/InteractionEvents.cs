using System;
using UnityEngine;

public class InteractionEvents
{
    public event Action<Interactable> OnInteract;

    public void Interact(Interactable interactable)
    {
        if (OnInteract != null)
            OnInteract(interactable);
    }
}
