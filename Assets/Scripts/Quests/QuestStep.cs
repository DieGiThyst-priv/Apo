using System;
using UnityEngine;

public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private string questName;

    public void InitialiseQuestStep(string questId, string displayName)
    {
        this.questId = questId;
        this.questName = displayName;
        UpdateState();
    }


    public static event Action<string, string> OnQuestStepUpdated;


    protected void UpdateState(string status = "")
    {
        string questName = questId;
        string objective = GetObjectiveText();

        OnQuestStepUpdated?.Invoke(questName, objective);
    }

    protected abstract string GetObjectiveText();

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
