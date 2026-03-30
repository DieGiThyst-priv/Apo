using UnityEngine;

public class TalkWithCompanions : QuestStep
{
    private int companionsFound = 0;
    private int companionsToFind = 2;

    private void OnEnable()
    {
        GamesEventsManager.Instance.interactionEvents.OnInteract += HandleInteraction;
    }

    private void OnDisable()
    {
        GamesEventsManager.Instance.interactionEvents.OnInteract -= HandleInteraction;
    }

    private void HandleInteraction(Interactable target)
    {

        if (companionsFound < companionsToFind)
        {
            companionsFound++;
        }

        if (companionsFound >= companionsToFind)
        {
            FinishQuestStep();
        }
    }
}