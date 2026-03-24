using UnityEngine;

[CreateAssetMenu(fileName = "ConversationNode", menuName = "Scriptable Objects/ConversationNode")]
public class ConversationNode : ScriptableObject
{
    public string Text { get; set; }
    public GameObject Speaker { get; set; }
    public ConversationNode[] NextNodes { get; set; }
    public ConversationNode[] ResponseOptions { get; set; }

    public ConversationNode(string text, GameObject speaker, ConversationNode[] nextNodes, ConversationNode[] responseOptions)
    {
        Text = text;
        Speaker = speaker;
        NextNodes = nextNodes;
        ResponseOptions = responseOptions;
    }

    public ConversationNode nextNode(int optionIndex) {
        if (ResponseOptions.Length == 0) {
            return NextNodes[0];
        }
        else if (optionIndex < 0 || optionIndex >= ResponseOptions.Length){
            Debug.LogError("Invalid option index");
            return null;
        }
        else if (NextNodes.Length == 0) {
            return null;
        }

        return ResponseOptions[optionIndex];
    }
}
