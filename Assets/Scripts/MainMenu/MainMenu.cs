using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private enum MenuMode
    {
        LevelSelect,
        EndScreen
    }

    [System.Serializable]
    private struct LevelEntry
    {
        public string label;
        public string sceneName;
    }

    [Header("Mode")]
    [SerializeField]
    private MenuMode menuMode = MenuMode.LevelSelect;

    [Header("Menu Content")]
    [SerializeField]
    private string menuTitle = "Sokoban";

    [SerializeField]
    private string subtitle = "Choose a level";

    [SerializeField]
    private string returnSceneName = "StartScreen";

    [SerializeField]
    private LevelEntry[] levels =
    {
        new LevelEntry { label = "Level 1", sceneName = "" },
        new LevelEntry { label = "Level 2", sceneName = "" },
        new LevelEntry { label = "Level 3", sceneName = "" }
    };

    [Header("Controls")]
    [SerializeField]
    private KeyCode quickStartKey = KeyCode.Space;

    private static MainMenu activeMenu;

    private readonly List<LevelEntry> resolvedLevels = new List<LevelEntry>();

    private void Awake()
    {
        if (activeMenu != null && activeMenu != this)
        {
            enabled = false;
            return;
        }

        activeMenu = this;
        ResolveLevels();
        BuildMenuUi();
    }

    private void OnDestroy()
    {
        if (activeMenu == this)
        {
            activeMenu = null;
        }
    }

    private void Update()
    {
        if (menuMode == MenuMode.EndScreen)
        {
            if (Input.GetKeyDown(quickStartKey))
            {
                LoadReturnScene();
            }
            return;
        }

        if (resolvedLevels.Count == 0)
        {
            return;
        }

        if (Input.GetKeyDown(quickStartKey))
        {
            LoadLevel(0);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            LoadLevel(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            LoadLevel(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            LoadLevel(2);
        }
    }

    public void StartGame()
    {
        LoadLevel(0);
    }

    public void ReturnToStart()
    {
        LoadReturnScene();
    }

    public void LoadLevel1()
    {
        LoadLevel(0);
    }

    public void LoadLevel2()
    {
        LoadLevel(1);
    }

    public void LoadLevel3()
    {
        LoadLevel(2);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= resolvedLevels.Count)
        {
            return;
        }

        string sceneName = resolvedLevels[levelIndex].sceneName;
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void ResolveLevels()
    {
        resolvedLevels.Clear();

        for (int index = 0; index < levels.Length && resolvedLevels.Count < 3; index++)
        {
            if (IsSceneLoadable(levels[index].sceneName))
            {
                resolvedLevels.Add(new LevelEntry
                {
                    label = string.IsNullOrWhiteSpace(levels[index].label) ? $"Level {resolvedLevels.Count + 1}" : levels[index].label,
                    sceneName = levels[index].sceneName
                });
            }
        }
    }

    private void BuildMenuUi()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        Transform existingRoot = canvas.transform.Find("GeneratedMainMenu");
        if (existingRoot != null)
        {
            return;
        }

        GameObject root = CreateUiObject("GeneratedMainMenu", canvas.transform);
        RectTransform rootRect = root.GetComponent<RectTransform>();
        StretchToParent(rootRect);

        GameObject panel = CreateUiObject("Panel", root.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(520f, 520f);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.11f, 0.16f, 0.23f, 0.98f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(40, 40, 40, 40);
        layout.spacing = 18f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateText("Title", panel.transform, menuTitle, 44f, FontStyles.Bold);

        if (menuMode == MenuMode.EndScreen)
        {
            CreateText("Subtitle", panel.transform, subtitle, 22f, FontStyles.Normal);
            CreateLevelButton(panel.transform, "Press Space", true, -1);
            return;
        }

        for (int index = 0; index < 3; index++)
        {
            bool hasLevel = index < resolvedLevels.Count;
            string label = hasLevel ? resolvedLevels[index].label : $"Level {index + 1} (Unavailable)";
            CreateLevelButton(panel.transform, $"{label}", hasLevel, index);
        }
    }

    private void CreateLevelButton(Transform parent, string label, bool interactable, int levelIndex)
    {
        GameObject buttonObject = CreateUiObject($"Level{levelIndex + 1}Button", parent);
        LayoutElement layoutElement = buttonObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = 72f;

        Image image = buttonObject.AddComponent<Image>();
        image.color = interactable ? new Color(0.23f, 0.52f, 0.83f, 1f) : new Color(0.28f, 0.31f, 0.35f, 0.9f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.interactable = interactable;
        if (levelIndex < 0)
        {
            button.onClick.AddListener(ReturnToStart);
        }
        else
        {
            button.onClick.AddListener(() => LoadLevel(levelIndex));
        }

        CreateText("Label", buttonObject.transform, label, 24f, FontStyles.Bold);
    }

    private TMP_Text CreateText(string objectName, Transform parent, string content, float fontSize, FontStyles fontStyle)
    {
        GameObject textObject = CreateUiObject(objectName, parent);
        LayoutElement layoutElement = textObject.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = fontSize > 30f ? 72f : 56f;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        text.enableWordWrapping = true;

        RectTransform rectTransform = text.rectTransform;
        StretchToParent(rectTransform);
        rectTransform.sizeDelta = new Vector2(0f, layoutElement.preferredHeight);
        return text;
    }

    private static GameObject CreateUiObject(string objectName, Transform parent)
    {
        GameObject gameObject = new GameObject(objectName, typeof(RectTransform));
        gameObject.transform.SetParent(parent, false);
        return gameObject;
    }

    private static bool IsSceneLoadable(string sceneName)
    {
        return !string.IsNullOrWhiteSpace(sceneName) && Application.CanStreamedLevelBeLoaded(sceneName);
    }

    private void LoadReturnScene()
    {
        if (!IsSceneLoadable(returnSceneName))
        {
            return;
        }

        SceneManager.LoadScene(returnSceneName);
    }

    private static void StretchToParent(RectTransform rectTransform)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
    }
}
