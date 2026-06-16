#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class StorySceneBuilder
{
    private const string ScenePath = "Assets/Scenes/StoryGame.unity";

    [MenuItem("Mafia/Build Story Scene")]
    public static void BuildFromMenu()
    {
        BuildScene();
    }

    public static void BuildScene()
    {
        EnsureTmpEssentials();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Camera camera = CreateCamera();
        EventSystem eventSystem = CreateEventSystem();
        Canvas canvas = CreateCanvas(out RectTransform root);

        Image background = CreateBackground(root);
        TextMeshProUGUI titleText = CreateTitle(root);
        TextMeshProUGUI storyText = CreateStoryText(root);
        TextMeshProUGUI historyText = CreateHistoryText(root);
        (Button buttonA, TextMeshProUGUI buttonAText) = CreateChoiceButton(root, "ButtonA", "A", new Vector2(-260f, -360f), new Color(0.78f, 0.16f, 0.16f));
        (Button buttonB, TextMeshProUGUI buttonBText) = CreateChoiceButton(root, "ButtonB", "B", new Vector2(260f, -360f), new Color(0.16f, 0.35f, 0.78f));

        GameManager gameManager = CreateGameManager(
            storyText,
            historyText,
            buttonA,
            buttonAText,
            buttonB,
            buttonBText);

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        EditorSceneManager.SaveScene(scene, ScenePath);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(ScenePath, true)
        };

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Story scene saved to {ScenePath}");
    }

    private static void EnsureTmpEssentials()
    {
        if (TMP_Settings.instance != null && TMP_Settings.defaultFontAsset != null)
        {
            return;
        }

        TMP_PackageResourceImporter.ImportResources(true, false, false);
    }

    private static Camera CreateCamera()
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.07f, 0.07f, 0.09f);
        camera.orthographic = true;
        cameraObject.tag = "MainCamera";
        cameraObject.AddComponent<AudioListener>();
        return camera;
    }

    private static EventSystem CreateEventSystem()
    {
        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
        return eventSystemObject.GetComponent<EventSystem>();
    }

    private static Canvas CreateCanvas(out RectTransform root)
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        root = canvasObject.GetComponent<RectTransform>();
        return canvas;
    }

    private static Image CreateBackground(RectTransform parent)
    {
        GameObject backgroundObject = CreateUiObject("Background", parent);
        RectTransform rect = backgroundObject.GetComponent<RectTransform>();
        StretchFull(rect);

        Image image = backgroundObject.AddComponent<Image>();
        image.color = new Color(0.09f, 0.09f, 0.11f);
        return image;
    }

    private static TextMeshProUGUI CreateTitle(RectTransform parent)
    {
        GameObject titleObject = CreateUiObject("Title", parent);
        RectTransform rect = titleObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -40f);
        rect.sizeDelta = new Vector2(1600f, 80f);

        TextMeshProUGUI text = titleObject.AddComponent<TextMeshProUGUI>();
        text.text = "Mafia Friz x KFC 2";
        text.fontSize = 42f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.95f, 0.82f, 0.35f);
        return text;
    }

    private static TextMeshProUGUI CreateStoryText(RectTransform parent)
    {
        GameObject storyObject = CreateUiObject("StoryText", parent);
        RectTransform rect = storyObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(0f, 120f);
        rect.sizeDelta = new Vector2(1500f, 420f);

        TextMeshProUGUI text = storyObject.AddComponent<TextMeshProUGUI>();
        text.text = "Ladowanie historii...";
        text.fontSize = 34f;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private static TextMeshProUGUI CreateHistoryText(RectTransform parent)
    {
        GameObject historyObject = CreateUiObject("HistoryText", parent);
        RectTransform rect = historyObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 220f);
        rect.sizeDelta = new Vector2(1500f, 180f);

        TextMeshProUGUI text = historyObject.AddComponent<TextMeshProUGUI>();
        text.text = "Sciezka: brak wyborow.";
        text.fontSize = 22f;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.color = new Color(0.75f, 0.75f, 0.78f);
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private static (Button, TextMeshProUGUI) CreateChoiceButton(
        RectTransform parent,
        string objectName,
        string prefix,
        Vector2 anchoredPosition,
        Color color)
    {
        GameObject buttonObject = CreateUiObject(objectName, parent);
        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(620f, 110f);

        Image image = buttonObject.AddComponent<Image>();
        image.color = color;

        Button button = buttonObject.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.1f;
        colors.pressedColor = color * 0.85f;
        colors.selectedColor = color;
        button.colors = colors;

        GameObject labelObject = CreateUiObject("Label", rect);
        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        StretchFull(labelRect);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = prefix;
        label.fontSize = 28f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        label.textWrappingMode = TextWrappingModes.Normal;

        return (button, label);
    }

    private static GameManager CreateGameManager(
        TextMeshProUGUI storyText,
        TextMeshProUGUI historyText,
        Button buttonA,
        TextMeshProUGUI buttonAText,
        Button buttonB,
        TextMeshProUGUI buttonBText)
    {
        GameObject managerObject = new GameObject("GameManager");
        GameManager gameManager = managerObject.AddComponent<GameManager>();
        gameManager.storyText = storyText;
        gameManager.historyText = historyText;
        gameManager.buttonA = buttonA;
        gameManager.buttonAText = buttonAText;
        gameManager.buttonB = buttonB;
        gameManager.buttonBText = buttonBText;
        return gameManager;
    }

    private static GameObject CreateUiObject(string name, Transform parent)
    {
        GameObject uiObject = new GameObject(name, typeof(RectTransform));
        uiObject.transform.SetParent(parent, false);
        return uiObject;
    }

    private static void StretchFull(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
#endif
