using TMPro;
using UnityEngine;

public class OptionsScript : MonoBehaviour
{
    [SerializeField] GameObject Options;
    [SerializeField] GameObject Button;
    private ConversationNode node;

    public void setNode(ConversationNode node) {
        this.node = node;
    }

    public void OnClicked() {
        this.Options.GetComponent<OptionsMaker>().SelectOption(node);
    }

    public void updateUI()
    {
        this.GetComponent<TextMeshProUGUI>().text = this.node.Text;
    }
}
