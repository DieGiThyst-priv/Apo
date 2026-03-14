using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Interactable currentInteractable;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();

        if (interactable != null)
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interactable interactable = collision.GetComponent<Interactable>();

        if (interactable != null && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }

    public void OnInteract()
    {
        if (currentInteractable != null)
        {
            currentInteractable.Interact(gameObject);
        }
    }
}