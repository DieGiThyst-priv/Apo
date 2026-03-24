using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConversationMaker : MonoBehaviour
{
    private GameObject[] presentCompanions;
    private string conversationFile;
    [SerializeField] private GameObject MC;
    private GameObject CH;
    void Start()
    {
        presentCompanions = GameObject.FindGameObjectsWithTag("Companion");
        TextAsset file = Resources.Load<TextAsset>("Lines/ScriptScene1");
        if (file != null)
        {
            conversationFile = file.text;
            Debug.Log("CONTENT:\n" + conversationFile);
        }
        else
        {
            Debug.LogError("TextAsset not found!");
        }
        CH = presentCompanions[0];
        CH.GetComponent<Conversable>().SetConversation(MakeConversation(conversationFile));
        if (CH.GetComponent<Conversable>().GetConversation() == null)
        {
            Debug.LogError("Companion no convers");
            return;
        }
    }

    void Update()
    {

    }

    public Conversation MakeConversation(string text)
    {
        List<ConversationNode> allNodes = new List<ConversationNode>();

        // Split into branches
        string[] branches = text.Split('$');

        Dictionary<string, ConversationNode> branchStarts = new Dictionary<string, ConversationNode>();

        for (int b = 0; b < branches.Length; b++)
        {
            string branch = branches[b].Trim();
            if (string.IsNullOrEmpty(branch)) continue;

            string branchId = (b == 0) ? "main" : branch.Substring(0, 1);
            string branchContent = (b == 0) ? branch : branch.Substring(1);

            string[] lines = branchContent.Split('>');

            ConversationNode prev = null;
            ConversationNode first = null;

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Parse speaker + text
                string speakerKey = "MC";
                string content = line;

                int colon = line.IndexOf(':');
                if (colon > 0)
                {
                    speakerKey = line.Substring(0, colon).Trim();
                    content = line.Substring(colon + 1).Trim();
                }

                GameObject speaker = speakerKey == "MC" ? MC : CH;

                // Create node
                ConversationNode node = ScriptableObject.CreateInstance<ConversationNode>();
                node.Init(content, speaker);

                allNodes.Add(node);

                if (first == null) first = node;

                if (prev != null)
                    prev.NextNodes = new[] { node };

                prev = node;
            }

            if (first != null)
                branchStarts[branchId] = first;
        }

        // 🔥 Link branches to main (VERY simple for now)
        if (branchStarts.ContainsKey("main"))
        {
            ConversationNode lastMain = branchStarts["main"];

            // walk to last node
            while (lastMain.NextNodes.Length > 0)
                lastMain = lastMain.NextNodes[0];

            // attach branches as response options
            List<ConversationNode> options = new List<ConversationNode>();

            foreach (var kvp in branchStarts)
            {
                if (kvp.Key == "main") continue;
                options.Add(kvp.Value);
            }

            lastMain.ResponseOptions = options.ToArray();
        }

        // Create conversation
        Conversation conversation = ScriptableObject.CreateInstance<Conversation>();
        conversation.Init(allNodes.ToArray());

        return conversation;
    }
}