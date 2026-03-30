using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questinfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool endPoint = true;


    private string questId;
    private QuestState currentState;

    private void Awake()
    {
        questId = questinfoForPoint.id;
    }

    private void OnEnable()
    {
        GamesEventsManager.Instance.questEvents.OnQuestStateChange += QuestStateChange;
        GamesEventsManager.Instance.interactionEvents.OnInteract += Interacted;
    }

    private void OnDisable()
    {
        GamesEventsManager.Instance.questEvents.OnQuestStateChange -= QuestStateChange;
        GamesEventsManager.Instance.interactionEvents.OnInteract -= Interacted;
    }


    private void Interacted(Interactable interactable)
    {
        if(interactable == null)
        {
            return;
        }

        Debug.Log("Interacted with : " +  interactable.name);
        if (currentState.Equals(QuestState.CAN_START) && startPoint)
        {
            GamesEventsManager.Instance.questEvents.StartQuest(questId);
        }
        else if(currentState.Equals(QuestState.CAN_FINISH) && endPoint){
            GamesEventsManager.Instance.questEvents.FinishQuest(questId);
        }

    }


    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentState = quest.state;
        }
    }
}
