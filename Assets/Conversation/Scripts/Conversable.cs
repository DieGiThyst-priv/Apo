using UnityEngine;

public class Conversable : Interactable
{
    [SerializeField] GameObject conversationManager;
    private Conversation conversation;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public override void Interact(GameObject interactor)
    {
        conversationManager.GetComponent<ConversationManager>().ProgressConversation(this.conversation,0);
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
