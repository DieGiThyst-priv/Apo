using UnityEngine;

public class Conversable : Interactable
{
    [SerializeField] GameObject conversationManager;
    private ConversationManager managerScript;
    private Conversation conversation;
    private CharacterStats characterStats;
    void Start()
    {
        characterStats = this.gameObject.GetComponent<CharacterStats>();
    }

    void Update()
    {
        
    }

    public override void Interact(GameObject interactor)
    {
        if (characterStats != null)
        {
            characterStats = this.gameObject.GetComponent<CharacterStats>();

        }
        Debug.Log("Interacted with consumed by conversable" + this.gameObject.name);
        if (this.gameObject.GetComponent<CharacterStats>().isCharacterDowned())
        {
            characterStats.Interact(interactor);
        }
        else
        {
            if (managerScript == null)
            {
                managerScript = conversationManager.GetComponent<ConversationManager>();
            }
            if (!managerScript.isConversationActiveCheck())
            {
                managerScript.ProgressConversation(this, 0);
            }
        }
    }

    public void clickToProgress() {
        if (managerScript.isConversationActiveCheck())
        {
            managerScript.ProgressConversation(this, 0);
        }
    }

    public void SetConversation(Conversation conversation)
    {
        this.conversation = conversation;
    }

    public Conversation GetConversation()
    {
        return conversation;
    }
}
