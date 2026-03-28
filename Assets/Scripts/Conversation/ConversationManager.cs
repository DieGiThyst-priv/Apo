using TMPro;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] GameObject conversationCanvas;
    [SerializeField] GameObject conversationTextGameObject;
    [SerializeField] GameObject nameTextGameObject;
    [SerializeField] GameObject speakerSpriteGameObject;
    [SerializeField] GameObject Options;
    [SerializeField] GameObject Player;
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
        ConversationNode currentNode = conversation.GetCurrentNode();

        if (currentNode.ResponseOptions != null && currentNode.ResponseOptions.Length > 0)
        {
            conversation.ProgressToNextNode(optionIndex);
            conversation.ProgressToNextNode(0);
            UpdateConversationUI(conversation);
            return;
        }

        if (currentNode.NextNodes.Length == 0)
        {
            EndConversation();
            return;
        }

        conversation.ProgressToNextNode(0);
        UpdateConversationUI(conversation);
    }

    public void SelectOption(ConversationNode selectedNode)
    {
        this.ProgressConversation(
            currentConversable,
            currentConversable.GetConversation().getIndexOfMatchingResponseNode(selectedNode)
        );
    }

    public void EndConversation()
    {
        conversationCanvas.SetActive(false);
        isConversationActive = false;
        Player.GetComponent<PlayerMovement>().setFrozen(false);
    }

    public void StartConversation(Conversable conversable)
    {
        Player.GetComponent<PlayerMovement>().setFrozen(true);
        isConversationActive = true;
        conversationCanvas.SetActive(true);
        currentConversable = conversable;
        if (currentConversable == null)
        {
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
        if (currentNode.ResponseOptions != null && currentNode.ResponseOptions.Length > 0)
        {
            OptionsMaker options = Options.GetComponent<OptionsMaker>();
            options.ShowOptions();
            populateOptions(currentNode.ResponseOptions);

            conversationText.text = ""; 
            nameText.text = currentNode.ResponseOptions[0].Speaker.name;
            speakerSprite.sprite = currentNode.ResponseOptions[0].Speaker.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            Options.GetComponent<OptionsMaker>().HideOptions();

            conversationText.text = currentNode.Text;
            nameText.text = currentNode.Speaker.name;
            speakerSprite.sprite = currentNode.Speaker.GetComponent<SpriteRenderer>().sprite;
        }
    }

    public void populateOptions(ConversationNode[] nodes) {

        Options.GetComponent<OptionsMaker>().populateOptions(nodes);
    }

    public bool isConversationActiveCheck() {
        return this.isConversationActive;
    }
}
