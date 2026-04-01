using UnityEngine;

public class QuestDoor : QuestPoint
{
    [SerializeField] private MapTransition transition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (currentState != QuestState.CAN_FINISH &&
            currentState != QuestState.FINISHED)
        {
            Debug.Log("I dont want to leave yet...");
            return;
        }

        transition.TriggerTransition(collision.gameObject);
    }
}
