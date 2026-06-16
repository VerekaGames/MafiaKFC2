using System.Collections.Generic;

[System.Serializable]
public class StoryNode
{
    public int id;
    public string text;
    public bool isEnding;

    public string choiceAText;
    public int choiceANodeId;

    public string choiceBText;
    public int choiceBNodeId;
}

[System.Serializable]
public class StoryWrapper
{
    public List<StoryNode> nodes;
}
