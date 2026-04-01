using TMPro;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    private void OnEnable()
    {
        QuestStep.OnQuestStepUpdated += UpdateUI;
    }

    private void OnDisable()
    {
        QuestStep.OnQuestStepUpdated -= UpdateUI;
    }

    private void UpdateUI(string questName, string objective)
    {
        questNameText.text = questName;
        objectiveText.text = objective;
    }
}
