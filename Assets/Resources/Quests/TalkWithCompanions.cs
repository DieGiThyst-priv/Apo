using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TalkWithCompanions;

public class TalkWithCompanions : QuestStep
{
    private int companionsToFind = 2;
    [SerializeField] private string companion1;
    [SerializeField] private string companion2;

    private HashSet<string> foundCompanions = new HashSet<string>();


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
        string name = target.gameObject.GetComponent<CharacterStats>().getCharacterName();
        if(name == null) return;
        Debug.Log("Call me " + name);

        if (foundCompanions.Contains(name)) return;

        if (name == companion1 || name == companion2)
        {
                foundCompanions.Add(name);
                UpdateState();
        }

        if (foundCompanions.Count >= companionsToFind)
        {
                Debug.Log("Found all companions in this scene!");
                FinishQuestStep();
        }
       

    }


    protected override string GetObjectiveText()
    {
        return $"Companions found ({foundCompanions.Count}/{companionsToFind})";
    }
}