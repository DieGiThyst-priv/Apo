using TMPro;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] GameObject conversationCanvas;
    [SerializeField] GameObject conversationTextGameObject;
    [SerializeField] GameObject nameTextGameObject;
    [SerializeField] GameObject speakerSpriteGameObject;
    private Conversable currentConversable;
    private bool isConversationActive;

    void Start()
    {
        conversationCanvas.SetActive(false);
        isConversationActive = false;
    }

    void Update()
    {
        
    }

    public void ProgressConversation(Conversable conversable, int optionIndex)
    {
        if (!isConversationActive)
        {
            StartConversation(conversable);
            return;
        }
        Conversation conversation = conversable.GetConversation();
        if (conversation.GetCurrentNode().NextNodes.Length == 0)
        {
            EndConversation();
            return;
        }
        conversation.ProgressToNextNode(optionIndex);
        UpdateConversationUI(conversation);
    }

    public void EndConversation()
    {
        conversationCanvas.SetActive(false);
        isConversationActive = false;
    }

    public void StartConversation(Conversable conversable)
    {
        isConversationActive = true;
        conversationCanvas.SetActive(true);
        currentConversable = conversable;
        if (currentConversable == null)
        {
            Debug.LogError("ConversationManager: StartConversation called with null conversable");
            return;
        }
        UpdateConversationUI(currentConversable.GetConversation());
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
