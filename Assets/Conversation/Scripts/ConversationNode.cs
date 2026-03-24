using UnityEngine;

[CreateAssetMenu(fileName = "ConversationNode", menuName = "Scriptable Objects/ConversationNode")]
public class ConversationNode : ScriptableObject
{
    public string Text;
    public GameObject Speaker;
    public ConversationNode[] NextNodes;
    public ConversationNode[] ResponseOptions;

    public void Init(string text, GameObject speaker)
    {
        Text = text;
        Speaker = speaker;
        NextNodes = new ConversationNode[0];
        ResponseOptions = new ConversationNode[0];
    }

    public ConversationNode NextNode(int optionIndex)
    {
        if (ResponseOptions != null && ResponseOptions.Length > 0)
        {
            if (optionIndex < 0 || optionIndex >= ResponseOptions.Length)
            {
                Debug.LogError("Invalid option index");
                return null;
            }
            return ResponseOptions[optionIndex];
        }

        if (NextNodes != null && NextNodes.Length > 0)
            return NextNodes[0];

        return null;
    }
}