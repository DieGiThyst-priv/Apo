using UnityEngine;

public abstract class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questinfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool endPoint = true;


    protected string questId;
    protected QuestState currentState;

    private void Awake()
    {
        questId = questinfoForPoint.id;
    }

    private void OnEnable()
    {
        GamesEventsManager.Instance.questEvents.OnQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        GamesEventsManager.Instance.questEvents.OnQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest)
    {
        if (quest.info.id.Equals(questId))
        {
            currentState = quest.state;
        }
    }
}
