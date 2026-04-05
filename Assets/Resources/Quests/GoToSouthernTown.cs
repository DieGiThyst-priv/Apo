using UnityEngine;

public class GoToSouthernTown : QuestStep
{
    protected override string GetObjectiveText()
    {
        return "Reach the southern town";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("SouthernTown")) return;

        Debug.Log("Reached the southern town!");
        FinishQuestStep();
    }

}
