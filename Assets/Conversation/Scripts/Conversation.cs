using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Scriptable Objects/Conversation")]
public class Conversation : ScriptableObject
{
    private ConversationNode currentNode;
    private ConversationNode[] nodes;

    public void Init(ConversationNode[] nodes)
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
        currentNode = currentNode.NextNode(optionIndex);
    }
}