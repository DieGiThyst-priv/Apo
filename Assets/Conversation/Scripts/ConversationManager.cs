using TMPro;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] GameObject conversationCanvas;
    [SerializeField] GameObject conversationTextGameObject;
    [SerializeField] GameObject nameTextGameObject;
    [SerializeField] GameObject speakerSpriteGameObject;
    private Conversable currentConversable;

    void Start()
    {
        conversationCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ProgressConversation(Conversation conversation, int optionIndex)
    {
        this.StartConversation(currentConversable);
        conversation.ProgressToNextNode(optionIndex);
        UpdateConversationUI(conversation);
        EndConversation();
    }

    public void EndConversation()
    {
        conversationCanvas.SetActive(false);
    }

    public void StartConversation(Conversable conversable)
    {
        conversationCanvas.SetActive(true);
        this.currentConversable = conversable;
        //UpdateConversationUI(currentConversable.GetConversation());
    }

    public void UpdateConversationUI(Conversation conversation)
    {
        ConversationNode currentNode = conversation.GetCurrentNode();
        TextMeshProUGUI conversationText = conversationTextGameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = nameTextGameObject.GetComponent<TextMeshProUGUI>();
        SpriteRenderer speakerSprite = speakerSpriteGameObject.GetComponent<SpriteRenderer>();
        conversationText.text = currentNode.Text;
        nameText.text = currentNode.Speaker.name;
        speakerSprite.sprite = currentNode.Speaker.GetComponent<SpriteRenderer>().sprite;
    }
}
