using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;


    private void Update()
    {
        foreach(Quest quest in questMap.Values)
        {
            if(quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
        }
    }

    private void Start()
    {

        foreach (Quest quest in questMap.Values)
        {
            GamesEventsManager.Instance.questEvents.QuestStateChange(quest);
        }
        GamesEventsManager.Instance.questEvents.StartQuest("FindCompanionsForAdventure");
    }


    private void Awake()
    {
        questMap = CreateQuestMap();
        Quest quest = GetQuestById("FindCompanionsForAdventure");
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GamesEventsManager.Instance.questEvents.QuestStateChange(quest);
    }

    private void OnEnable()
    {
        GamesEventsManager.Instance.questEvents.OnStartQuest += StartQuest;
        GamesEventsManager.Instance.questEvents.OnAdvanceQuest += AdvanceQuest;
        GamesEventsManager.Instance.questEvents.OnFinishQuest += FinishQuest;
    }

    private void OnDisable()
    {
        GamesEventsManager.Instance.questEvents.OnStartQuest -= StartQuest;
        GamesEventsManager.Instance.questEvents.OnAdvanceQuest -= AdvanceQuest;
        GamesEventsManager.Instance.questEvents.OnFinishQuest -= FinishQuest;
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        bool meetsRequirements = true;

        foreach(QuestInfoSO prerequisitesQuestInfo in quest.info.questPrerequisites)
        {
            if(GetQuestById(prerequisitesQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }
        return meetsRequirements;
    }

   
    private void StartQuest(string id)
    {
        Debug.Log("Starting quest" + id);
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Debug.Log("Advancing quest" + id);
        Quest quest = GetQuestById(id);
        quest.MoveToNextStep();
        if (quest.CurrentStepExist())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Debug.Log("Finishing quest" + id);
        Quest quest = GetQuestById(id);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);

    }


    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfoSO questInfoSO in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfoSO.id))
            {
                Debug.LogWarning("Duplciate ID found when createing quest map: " + questInfoSO.id);

            }
            idToQuestMap.Add(questInfoSO.id, new Quest(questInfoSO));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogWarning("ID not found in the quest map: " + id);

        }
        return quest;
    }
}
