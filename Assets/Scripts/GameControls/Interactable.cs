using System;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();

    protected void RaiseInteraction()
    {
        GamesEventsManager.Instance.interactionEvents.Interact(this);
    }
}