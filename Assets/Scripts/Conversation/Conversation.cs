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

    public ConversationNode[] GetNextNodes()
    {
        return nodes;
    }

    public bool nextIsOptions() {
        return nodes.Length > 1;
    }

    public int getIndexOfMatchingResponseNode(ConversationNode selectedNode) {
        int iterator = -1;
        ConversationNode[] responses = currentNode.ResponseOptions;
        for (int i = 0; i < responses.Length; i++)
        {
            if (responses[i].Text == selectedNode.Text) {
                iterator = i;
                return iterator;
            }
        }
        return iterator;
    }
}