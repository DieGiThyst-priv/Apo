using UnityEngine;

public class Conversable : Interactable
{
    [SerializeField] GameObject conversationManager;
    private ConversationManager managerScript;
    private Conversation conversation;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Interact(GameObject interactor)
    {
        if (managerScript == null) {
            managerScript = conversationManager.GetComponent<ConversationManager>();
        }
        if (!managerScript.isConversationActiveCheck()) {
            managerScript.ProgressConversation(this, 0);
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
