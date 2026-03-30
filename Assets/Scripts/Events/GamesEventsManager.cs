using UnityEngine;

public class GamesEventsManager : MonoBehaviour
{
    public static GamesEventsManager Instance { get; private set; }

    public QuestEvents questEvents;
    public InteractionEvents interactionEvents;


    public void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("Found more than one instance of Events manager");
        }
        Instance = this;



        questEvents = new QuestEvents();
        interactionEvents = new InteractionEvents();
    }
}
