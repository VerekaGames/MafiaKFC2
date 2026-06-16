using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI historyText;

    public Button buttonA;
    public TextMeshProUGUI buttonAText;

    public Button buttonB;
    public TextMeshProUGUI buttonBText;

    [Header("Config")]
    public string resourceStoryFileName = "story";
    public int startingNodeId = 1;

    private readonly Dictionary<int, StoryNode> nodeDictionary = new Dictionary<int, StoryNode>();
    private readonly List<string> playerChoices = new List<string>();
    private StoryNode currentNode;

    private void Start()
    {
        buttonA.onClick.AddListener(OnChoiceA);
        buttonB.onClick.AddListener(OnChoiceB);

        if (!LoadStoryFromJSON())
        {
            SetFatalError("Blad: nie udalo sie zaladowac historii.");
            return;
        }

        DisplayNode(startingNodeId);
    }

    private bool LoadStoryFromJSON()
    {
        nodeDictionary.Clear();

        TextAsset jsonFile = Resources.Load<TextAsset>(resourceStoryFileName);
        if (jsonFile == null)
        {
            Debug.LogError($"Nie znaleziono pliku Resources/{resourceStoryFileName}.json");
            return false;
        }

        StoryWrapper wrapper = JsonUtility.FromJson<StoryWrapper>(jsonFile.text);
        if (wrapper == null || wrapper.nodes == null || wrapper.nodes.Count == 0)
        {
            Debug.LogError("JSON historii jest pusty lub ma zly format.");
            return false;
        }

        foreach (StoryNode node in wrapper.nodes)
        {
            if (nodeDictionary.ContainsKey(node.id))
            {
                Debug.LogWarning($"Zduplikowane ID wezla: {node.id}. Pomijam duplikat.");
                continue;
            }

            nodeDictionary.Add(node.id, node);
        }

        return true;
    }

    private void DisplayNode(int nodeId)
    {
        if (!nodeDictionary.TryGetValue(nodeId, out currentNode))
        {
            Debug.LogError($"Nie znaleziono wezla o ID: {nodeId}");
            SetFatalError("Blad scenariusza: brak wskazanego wezla.");
            return;
        }

        storyText.text = currentNode.text;
        RefreshHistoryText();

        if (currentNode.isEnding)
        {
            ShowEndingUI();
            return;
        }

        ShowChoiceUI();
    }

    private void ShowChoiceUI()
    {
        buttonA.gameObject.SetActive(true);
        buttonB.gameObject.SetActive(true);

        buttonAText.text = currentNode.choiceAText;
        buttonBText.text = currentNode.choiceBText;
    }

    private void ShowEndingUI()
    {
        buttonA.gameObject.SetActive(true);
        buttonB.gameObject.SetActive(false);

        buttonAText.text = "Zagraj ponownie";
    }

    private void OnChoiceA()
    {
        if (currentNode == null)
        {
            return;
        }

        if (currentNode.isEnding)
        {
            RestartGame();
            return;
        }

        playerChoices.Add($"A: {currentNode.choiceAText}");
        DisplayNode(currentNode.choiceANodeId);
    }

    private void OnChoiceB()
    {
        if (currentNode == null || currentNode.isEnding)
        {
            return;
        }

        playerChoices.Add($"B: {currentNode.choiceBText}");
        DisplayNode(currentNode.choiceBNodeId);
    }

    private void RestartGame()
    {
        playerChoices.Clear();
        DisplayNode(startingNodeId);
    }

    private void RefreshHistoryText()
    {
        if (historyText == null)
        {
            return;
        }

        if (playerChoices.Count == 0)
        {
            historyText.text = "Sciezka: brak wyborow.";
            return;
        }

        historyText.text = "Sciezka:\n- " + string.Join("\n- ", playerChoices);
    }

    private void SetFatalError(string message)
    {
        storyText.text = message;
        buttonA.gameObject.SetActive(false);
        buttonB.gameObject.SetActive(false);
    }
}
