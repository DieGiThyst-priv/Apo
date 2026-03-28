using System.Collections;
using TMPro;
using UnityEngine;

public class OptionsMaker : MonoBehaviour
{
    [SerializeField] private System.Collections.Generic.List<GameObject> optionsArray;
    [SerializeField] private GameObject Scrollbar;
    [SerializeField] private GameObject OptionsElements;
    [SerializeField] private GameObject ConversationManager;
    private ConversationNode[] currentOptions;

    private void Start()
    {
        this.OptionsElements.SetActive(false);
    }

    public void ShowOptions() {
        this.OptionsElements.SetActive(true);
    }

    public void HideOptions() {
        this.OptionsElements.SetActive(false);
    }

    public void SelectOption(ConversationNode selectedNode) {
        this.HideOptions();
        ConversationManager.GetComponent<ConversationManager>().SelectOption(selectedNode);
    }

    public void populateOptions(ConversationNode[] options)
    {
        currentOptions = options;
        int currentScroll = Scrollbar.GetComponent<Scroll>().getCurrentScroll();

        for (int i = 0; i < optionsArray.Count; i++)
        {
            GameObject optionObject = optionsArray[i];

            if (i + currentScroll < currentOptions.Length)
            {
                optionObject.SetActive(true);
                optionObject.GetComponent<OptionsScript>()
                    .setNode(currentOptions[i + currentScroll]);
                optionObject.GetComponent<OptionsScript>().updateUI();
            }
            else
            {
                optionObject.transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
