using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;

    public void InitialiseQuestStep(string questId)
    {
        this.questId = questId;
    }

    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;


            GamesEventsManager.Instance.questEvents.AdvanceQuest(questId);


            Destroy(this.gameObject);
        }
    }  
}
