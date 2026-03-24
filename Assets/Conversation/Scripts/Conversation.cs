using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Scriptable Objects/Conversation")]
public class Conversation : ScriptableObject
{
    private ConversationNode currentNode;
    private ConversationNode[] nodes;


    public Conversation(ConversationNode[] nodes)
    {
        this.nodes = nodes;
        currentNode = nodes[0];
    }

    public ConversationNode GetCurrentNode()
    {
        return currentNode;
    }

    public void ProgressToNextNode(int optionIndex)
    {
        if (optionIndex < 0 || optionIndex >= currentNode.ResponseOptions.Length)
        {
            currentNode = currentNode.nextNode(0);
        }

        currentNode = currentNode.nextNode(optionIndex);
    }
}
