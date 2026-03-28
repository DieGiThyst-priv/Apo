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
    // Tokeniser
    // ---------------------------------------------------------------

    private enum TokenType { Line, BranchMarker, OpenBlock }

    private struct Token
    {
        public TokenType Type;
        public string Raw;
        public int BlockId;
        public int BranchNum;
        public string LineText;
    }

    private bool IsBlockMarker(string line, out int blockId)
    {
        blockId = -1;
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

            if (IsBlockMarker(line, out int bid))
            {
                tokens.Add(new Token { Type = TokenType.OpenBlock, Raw = line, BlockId = bid });
                continue;
            }

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
    // Returns (first node, last node, loose branch tails still needing wiring)
    // Stops before a BranchMarker or a closing £ownerBlockId token.
    // ---------------------------------------------------------------

    private (ConversationNode first, ConversationNode last, List<ConversationNode> loose)
        ParseSegment(int ownerBlockId = -1)
    {
        ConversationNode head = null;
        ConversationNode tail = null;
        List<ConversationNode> pendingTails = null;

        void Append(ConversationNode n)
        {
            // Wire any floating branch tails to this new node
            if (pendingTails != null)
            {
                foreach (var bt in pendingTails)
                    if (bt.ResponseOptions == null || bt.ResponseOptions.Length == 0)
                        bt.NextNodes = new[] { n };
                pendingTails = null;
            }

            if (head == null) { head = n; tail = n; return; }
            tail.NextNodes = new[] { n };
            tail = n;
        }

        while (_pos < _tokens.Count)
        {
            Token tok = _tokens[_pos];

            // £N — either closes our owner block, or opens a nested choice
            if (tok.Type == TokenType.OpenBlock)
            {
                if (tok.BlockId == ownerBlockId) break; // leave for caller to consume

                _pos++; // consume open £N
                (ConversationNode carrier, List<ConversationNode> bTails) =
                    ParseChoiceBlock(tok.BlockId);

                if (tail != null)
                {
                    // Absorb choice onto the last real spoken node
                    tail.ResponseOptions = carrier.ResponseOptions;
                    tail.NextNodes = new ConversationNode[0];
                }
                else
                {
                    // Choice is the very first thing in this segment
                    head = carrier;
                    tail = carrier;
                }

                pendingTails = bTails;
                continue;
            }

            // $ — belongs to our caller's choice block, stop
            if (tok.Type == TokenType.BranchMarker) break;

            // Plain dialogue line
            if (tok.Type == TokenType.Line)
            {
                _pos++;
                foreach (string beat in tok.LineText.Split('>'))
                {
                    string b = beat.Trim();
                    if (!string.IsNullOrEmpty(b)) Append(MakeNode(b));
                }
                continue;
            }

            break;
        }

        return (head, tail, pendingTails);
    }

    // ---------------------------------------------------------------
    // ParseChoiceBlock
    // Called after consuming the opening £bid token.
    // Collects branches; returns carrier node + all loose branch tails.
    // Does NOT parse the continuation — that is left to ParseSegment
    // so the continuation is wired naturally via pendingTails.
    // ---------------------------------------------------------------

    private (ConversationNode carrier, List<ConversationNode> allLooseTails)
        ParseChoiceBlock(int bid)
    {
        var branchHeads = new List<ConversationNode>();
        var allLooseTails = new List<ConversationNode>();

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
                (ConversationNode bFirst, ConversationNode bLast, List<ConversationNode> loose) =
                    ParseSegment(bid);

                if (bFirst != null) branchHeads.Add(bFirst);
                if (bLast != null) allLooseTails.Add(bLast);
                if (loose != null) allLooseTails.AddRange(loose);
                continue;
            }

            _pos++; // skip unexpected tokens
        }

        ConversationNode carrier = ScriptableObject.CreateInstance<ConversationNode>();
        carrier.Init("__choice__", null);
        carrier.ResponseOptions = branchHeads.ToArray();
        carrier.NextNodes = new ConversationNode[0];

        return (carrier, allLooseTails);
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

    // ---------------------------------------------------------------
    // Public entry point
    // ---------------------------------------------------------------

    public Conversation MakeConversation(string text)
    {
        _tokens = Tokenise(text);
        _pos = 0;

        (ConversationNode root, _, _) = ParseSegment();

        var all = new List<ConversationNode>();
        var visited = new HashSet<ConversationNode>();
        CollectAll(root, all, visited);

        Conversation conv = ScriptableObject.CreateInstance<Conversation>();
        conv.Init(all.ToArray());
        return conv;
    }
}