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
        CH.GetComponent<Conversable>().SetConversation(MakeConversationNodes(conversationFile));
        if (CH.GetComponent<Conversable>().GetConversation() == null)
        {
            Debug.LogError("Companion no convers");
            return;
        }
    }

    void Update()
    {

    }

    public Conversation MakeConversationNodes(string text)
    {
        // Split by $ but keep everything, including main branch
        string[] parts = text.Split(new[] { "$" }, System.StringSplitOptions.None);

        Dictionary<string, ConversationNode> branchNodes = new Dictionary<string, ConversationNode>();
        int branchCounter = 0;

        foreach (string rawBranch in parts)
        {
            string branch = rawBranch.Trim();
            if (string.IsNullOrEmpty(branch)) continue;

            string branchId = null;
            string branchContent = branch;

            // If branch starts with a number (like 1MC:...), extract numeric branchId
            int firstChar = branch[0];
            if (char.IsDigit((char)firstChar))
            {
                int colonIndex = branch.IndexOfAny(new char[] { 'M', 'C' }); // find where speaker prefix starts
                if (colonIndex > 0)
                {
                    branchId = branch.Substring(0, colonIndex).Trim();
                    branchContent = branch.Substring(colonIndex).Trim();
                }
            }
            else if (branchCounter == 0)
            {
                branchId = "main"; // first part without $ → main branch
            }

            // Split lines by >
            string[] lines = branchContent.Split('>');

            ConversationNode previousNode = null;
            ConversationNode firstNode = null;

            foreach (string lineRaw in lines)
            {
                string line = lineRaw.Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Parse speaker
                string speakerPrefix = null;
                string content = null;
                int colonIndex = line.IndexOf(':');
                if (colonIndex > 0)
                {
                    speakerPrefix = line.Substring(0, colonIndex).Trim();
                    content = line.Substring(colonIndex + 1).Trim();
                }
                else
                {
                    speakerPrefix = "MC"; // default
                    content = line;
                }

                // Map prefix to GameObject
                GameObject speakerGO = speakerPrefix == "MC" ? MC : CH;

                // Create node
                ConversationNode node = ScriptableObject.CreateInstance<ConversationNode>();
                node.Text = content;
                node.Speaker = speakerGO;
                node.NextNodes = new ConversationNode[0];
                node.ResponseOptions = new ConversationNode[0];

                if (firstNode == null) firstNode = node;
                if (previousNode != null)
                {
                    previousNode.NextNodes = new ConversationNode[] { node };
                }

                previousNode = node;
            }

            if (!string.IsNullOrEmpty(branchId) && firstNode != null)
            {
                branchNodes[branchId] = firstNode;
            }

            branchCounter++;
        }

        // Use main branch as starting node
        ConversationNode mainNode = branchNodes.ContainsKey("main") ? branchNodes["main"] : null;

        if (mainNode == null)
        {
            Debug.LogError("No main conversation found!");
            return null;
        }

        // Collect all nodes in a flat list for the Conversation constructor
        List<ConversationNode> allNodes = new List<ConversationNode>(branchNodes.Values);

        // Create the Conversation ScriptableObject
        Conversation conversation = ScriptableObject.CreateInstance<Conversation>();
        // Use reflection to set private nodes field since it's private, or you can make a constructor that accepts nodes
        var nodesField = typeof(Conversation).GetField("nodes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (nodesField != null)
        {
            nodesField.SetValue(conversation, allNodes.ToArray());
        }

        // Set starting node
        var currentNodeField = typeof(Conversation).GetField("currentNode", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (currentNodeField != null)
        {
            currentNodeField.SetValue(conversation, mainNode);
        }

        return conversation;
    }
}