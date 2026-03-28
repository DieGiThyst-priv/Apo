using System;
using System.Collections.Generic;
using UnityEngine;

public class ConversationMaker : MonoBehaviour
{
    private GameObject[] presentCompanions;
    private string conversationFile;
    [SerializeField] private GameObject MC;
    private GameObject CH;
    [SerializeField] private List<GameObject> allCharacters;

    private Dictionary<string, GameObject> characterMap = new Dictionary<string, GameObject>();

    void Start()
    {
        presentCompanions = GameObject.FindGameObjectsWithTag("Companion");
        CH = presentCompanions[0];
        characterMap["MC"] = MC;
        characterMap["CH"] = CH;

        foreach (var c in allCharacters)
        {
            string acronym = c.name.Substring(0, Math.Min(2, c.name.Length)).ToUpper();
            if (!characterMap.ContainsKey(acronym))
                characterMap[acronym] = c;
        }

        TextAsset file = Resources.Load<TextAsset>("Lines/ScriptScene1");
        if (file != null)
        {
            conversationFile = file.text;
            Debug.Log("CONTENT:\n" + conversationFile);
        }
        else
        {
            Debug.LogError("TextAsset not found!");
            return;
        }

        CH.GetComponent<Conversable>().SetConversation(MakeConversation(conversationFile));
        if (CH.GetComponent<Conversable>().GetConversation() == null)
            Debug.LogError("Companion has no conversation");
    }

    // ---------------------------------------------------------------
    // Token types
    // ---------------------------------------------------------------

    private enum TokenType { Line, BranchMarker, OpenBlock, CloseBlock }

    private struct Token
    {
        public TokenType Type;
        public string Raw;
        public int BlockId;
        public int BranchNum;
        public string LineText;
    }

    // ---------------------------------------------------------------
    // Tokeniser — handles £ mangled to \uFFFD by Unity file reading
    // ---------------------------------------------------------------

    private bool IsBlockMarker(string line, out int blockId)
    {
        blockId = -1;
        // Accept £ (U+00A3), its UTF-8-mangled form \uFFFD, or literal fallback
        if (line.Length < 2) return false;
        char first = line[0];
        if (first != '£' && first != '\uFFFD') return false;
        return int.TryParse(line.Substring(1).Trim(), out blockId);
    }

    private List<Token> Tokenise(string text)
    {
        var tokens = new List<Token>();
        string[] lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            // £N / \uFFFDN — open or close a choice block
            if (IsBlockMarker(line, out int bid))
            {
                tokens.Add(new Token { Type = TokenType.OpenBlock, Raw = line, BlockId = bid });
                continue;
            }

            // $N — branch marker
            if (line[0] == '$')
            {
                int i = 1;
                while (i < line.Length && char.IsDigit(line[i])) i++;
                if (int.TryParse(line.Substring(1, i - 1), out int bnum))
                {
                    tokens.Add(new Token { Type = TokenType.BranchMarker, Raw = line, BranchNum = bnum });
                    string rest = line.Substring(i).Trim();
                    if (!string.IsNullOrEmpty(rest))
                        tokens.Add(new Token { Type = TokenType.Line, Raw = rest, LineText = rest });
                }
                continue;
            }

            // Plain dialogue
            tokens.Add(new Token { Type = TokenType.Line, Raw = line, LineText = line });
        }

        return tokens;
    }

    // ---------------------------------------------------------------
    // Parser state
    // ---------------------------------------------------------------

    private List<Token> _tokens;
    private int _pos;

    // ---------------------------------------------------------------
    // ParseSegment
    // Returns (firstNode, lastNode) of the linear chain parsed.
    // Stops before a BranchMarker or a closing £bid for the caller.
    // ---------------------------------------------------------------

    private (ConversationNode first, ConversationNode last) ParseSegment(int ownerBlockId = -1)
    {
        ConversationNode head = null;
        ConversationNode tail = null;

        void Append(ConversationNode n)
        {
            if (head == null) { head = n; tail = n; return; }
            tail.NextNodes = new[] { n };
            tail = n;
        }

        while (_pos < _tokens.Count)
        {
            Token tok = _tokens[_pos];

            // £N — could be closing our ownerBlock, or opening a nested block
            if (tok.Type == TokenType.OpenBlock)
            {
                // Is this the close of the block our caller opened?
                if (tok.BlockId == ownerBlockId)
                    break; // leave the token for ParseChoiceBlock to consume

                // It's a new nested choice block — open it
                _pos++; // consume the open £N
                (ConversationNode choiceFirst, ConversationNode choiceLast) =
                    ParseChoiceBlock(tok.BlockId);

                // Wire: if we already have nodes, chain into the choice
                if (tail != null)
                {
                    // The choice result attaches to the last real node before it.
                    // We want that last node to hold ResponseOptions, not a gateway.
                    // ParseChoiceBlock returns the branch heads via choiceFirst.ResponseOptions.
                    tail.ResponseOptions = choiceFirst.ResponseOptions;
                    tail.NextNodes = new ConversationNode[0];
                    // choiceLast is the shared continuation node (or null)
                    // — we don't append it here; ParseChoiceBlock already wired branches to it.
                    // We need to continue appending AFTER the choice, so update tail
                    // to the continuation tail if it exists.
                    if (choiceLast != null)
                    {
                        tail.NextNodes = new ConversationNode[0]; // options only, no fallthrough
                        // Each branch tail already points to choiceLast's chain start.
                        // We now need OUR tail to become the end of the continuation chain.
                        tail = choiceLast;
                    }
                }
                else
                {
                    // Choice is the very first thing — keep the gateway as head temporarily
                    head = choiceFirst;
                    tail = choiceLast ?? choiceFirst;
                }
                continue;
            }

            // $ — belongs to our caller's choice block, stop
            if (tok.Type == TokenType.BranchMarker)
                break;

            // Plain dialogue line
            if (tok.Type == TokenType.Line)
            {
                _pos++;
                string[] beats = tok.LineText.Split('>');
                foreach (string beat in beats)
                {
                    string b = beat.Trim();
                    if (string.IsNullOrEmpty(b)) continue;
                    Append(MakeNode(b));
                }
                continue;
            }

            break;
        }

        return (head, tail);
    }

    // ---------------------------------------------------------------
    // ParseChoiceBlock
    // Called after consuming the opening £bid.
    // Returns a carrier node whose ResponseOptions = branch heads,
    // and the tail of the continuation chain (may be null).
    // ---------------------------------------------------------------

    private (ConversationNode carrier, ConversationNode continuationTail) ParseChoiceBlock(int bid)
    {
        var branchHeads = new List<ConversationNode>();
        var branchTails = new List<ConversationNode>();

        // ── Collect branches until closing £bid ──────────────────────
        while (_pos < _tokens.Count)
        {
            Token tok = _tokens[_pos];

            // Closing £bid
            if (tok.Type == TokenType.OpenBlock && tok.BlockId == bid)
            {
                _pos++; // consume close £bid
                break;
            }

            // Branch $N
            if (tok.Type == TokenType.BranchMarker)
            {
                _pos++; // consume $N
                (ConversationNode bFirst, ConversationNode bLast) = ParseSegment(bid);
                if (bFirst != null)
                {
                    branchHeads.Add(bFirst);
                    branchTails.Add(bLast ?? bFirst);
                }
                continue;
            }

            _pos++; // skip unexpected tokens
        }

        // ── Parse continuation after the closing £bid ─────────────────
        (ConversationNode contFirst, ConversationNode contLast) = ParseSegment();

        // ── Wire every branch tail → continuation start ───────────────
        if (contFirst != null)
        {
            foreach (var bt in branchTails)
            {
                // Only wire if branch tail has no options of its own
                if (bt.ResponseOptions == null || bt.ResponseOptions.Length == 0)
                    bt.NextNodes = new[] { contFirst };
            }
        }

        // ── Carrier: a node just to carry ResponseOptions ─────────────
        // This gets absorbed into the preceding real node by ParseSegment.
        ConversationNode carrier = ScriptableObject.CreateInstance<ConversationNode>();
        carrier.Init("__choice__", null);
        carrier.ResponseOptions = branchHeads.ToArray();
        carrier.NextNodes = new ConversationNode[0];

        return (carrier, contLast ?? contFirst);
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private ConversationNode MakeNode(string line)
    {
        string speakerKey = "MC";
        string content = line;

        int colon = line.IndexOf(':');
        if (colon > 0)
        {
            speakerKey = line.Substring(0, colon).Trim().ToUpper();
            content = line.Substring(colon + 1).Trim();
        }

        if (!characterMap.TryGetValue(speakerKey, out GameObject speaker))
            speaker = MC;

        ConversationNode node = ScriptableObject.CreateInstance<ConversationNode>();
        node.Init(content, speaker);
        return node;
    }

    private ConversationNode GetTail(ConversationNode head)
    {
        var visited = new HashSet<ConversationNode>();
        ConversationNode cur = head;
        while (cur.NextNodes != null && cur.NextNodes.Length > 0 && !visited.Contains(cur))
        {
            visited.Add(cur);
            cur = cur.NextNodes[0];
        }
        return cur;
    }

    // ---------------------------------------------------------------
    // Public entry point
    // ---------------------------------------------------------------

    public Conversation MakeConversation(string text)
    {
        _tokens = Tokenise(text);
        _pos = 0;

        (ConversationNode root, _) = ParseSegment();

        var all = new List<ConversationNode>();
        var visited = new HashSet<ConversationNode>();
        CollectAll(root, all, visited);

        Conversation conv = ScriptableObject.CreateInstance<Conversation>();
        conv.Init(all.ToArray());
        return conv;
    }

    private void CollectAll(ConversationNode node,
                            List<ConversationNode> list,
                            HashSet<ConversationNode> visited)
    {
        if (node == null || visited.Contains(node)) return;
        visited.Add(node);
        list.Add(node);

        if (node.NextNodes != null)
            foreach (var n in node.NextNodes) CollectAll(n, list, visited);
        if (node.ResponseOptions != null)
            foreach (var n in node.ResponseOptions) CollectAll(n, list, visited);
    }
}