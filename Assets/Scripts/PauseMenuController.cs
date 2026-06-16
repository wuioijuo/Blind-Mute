using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenuController : MonoBehaviour
{
    [Header("Pause Menu")]
    public KeyCode pauseKey = KeyCode.Escape;
    public string mainMenuSceneName = "MainMenu";

    [Header("Control Lock")]
    public bool disablePlayerControls = true;
    public bool pauseTime = true;

    private GameObject pauseCanvas;
    private bool isPaused;

    private readonly List<Behaviour> disabledComponents = new List<Behaviour>();
    private readonly Dictionary<Behaviour, bool> previousEnabledStates = new Dictionary<Behaviour, bool>();

    private CursorLockMode previousLockState;
    private bool previousCursorVisible;

    private static readonly string[] ControlComponentNames =
    {
        "PlayerController",
        "PlayerInteraction",
        "EchoAbility"
    };

    private void Awake()
    {
        // Important: the game must always start unpaused.
        isPaused = false;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        EnsureEventSystemExists();
        BuildPauseMenu();
        ClosePauseMenu(force: true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
            {
                ClosePauseMenu();
            }
            else
            {
                OpenPauseMenu();
            }
        }
    }

    private void BuildPauseMenu()
    {
        if (pauseCanvas != null)
        {
            Destroy(pauseCanvas);
        }

        pauseCanvas = new GameObject("PauseMenuCanvas_Runtime");
        pauseCanvas.transform.SetParent(transform, false);

        Canvas canvas = pauseCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = pauseCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        pauseCanvas.AddComponent<GraphicRaycaster>();

        GameObject background = CreateUIObject("Background", pauseCanvas.transform);
        Image backgroundImage = background.AddComponent<Image>();
        backgroundImage.color = new Color(0f, 0f, 0f, 0.78f);
        RectTransform backgroundRect = background.GetComponent<RectTransform>();
        StretchToFullScreen(backgroundRect);

        GameObject panel = CreateUIObject("PausePanel", pauseCanvas.transform);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.04f, 0.04f, 0.04f, 0.92f);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(620f, 520f);

        CreateText(panel.transform, "ПАУЗА", 52, new Vector2(0f, 170f), new Vector2(560f, 80f));
        CreateText(panel.transform, "Игра приостановлена", 24, new Vector2(0f, 110f), new Vector2(560f, 45f));

        CreateButton(panel.transform, "ПРОДОЛЖИТЬ", new Vector2(0f, 35f), ClosePauseMenu);
        CreateButton(panel.transform, "В ГЛАВНОЕ МЕНЮ", new Vector2(0f, -55f), GoToMainMenu);
        CreateButton(panel.transform, "ВЫЙТИ ИЗ ИГРЫ", new Vector2(0f, -145f), QuitGame);

        pauseCanvas.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        if (isPaused)
        {
            return;
        }

        isPaused = true;

        previousLockState = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseTime)
        {
            Time.timeScale = 0f;
        }

        if (disablePlayerControls)
        {
            SetPlayerControlsEnabled(false);
        }

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);
        }
    }

    public void ClosePauseMenu()
    {
        ClosePauseMenu(force: false);
    }

    private void ClosePauseMenu(bool force)
    {
        isPaused = false;

        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }

        if (pauseTime)
        {
            Time.timeScale = 1f;
        }

        RestorePlayerControls();

        if (!force)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Level starts as normal gameplay.
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        RestorePlayerControls();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void SetPlayerControlsEnabled(bool enabled)
    {
        if (enabled)
        {
            RestorePlayerControls();
            return;
        }

        disabledComponents.Clear();
        previousEnabledStates.Clear();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            player = GameObject.Find("Player");
        }

        if (player == null)
        {
            return;
        }

        MonoBehaviour[] behaviours = player.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == null || behaviour == this)
            {
                continue;
            }

            string typeName = behaviour.GetType().Name;
            if (IsControlComponent(typeName))
            {
                previousEnabledStates[behaviour] = behaviour.enabled;
                behaviour.enabled = false;
                disabledComponents.Add(behaviour);
            }
        }
    }

    private void RestorePlayerControls()
    {
        foreach (Behaviour behaviour in disabledComponents)
        {
            if (behaviour == null)
            {
                continue;
            }

            if (previousEnabledStates.TryGetValue(behaviour, out bool wasEnabled))
            {
                behaviour.enabled = wasEnabled;
            }
            else
            {
                behaviour.enabled = true;
            }
        }

        disabledComponents.Clear();
        previousEnabledStates.Clear();
    }

    private bool IsControlComponent(string typeName)
    {
        for (int i = 0; i < ControlComponentNames.Length; i++)
        {
            if (typeName == ControlComponentNames[i])
            {
                return true;
            }
        }

        return false;
    }

    private void EnsureEventSystemExists()
    {
        if (FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.SetParent(parent, false);
        obj.AddComponent<RectTransform>();
        return obj;
    }

    private void StretchToFullScreen(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private void CreateText(Transform parent, string textValue, int fontSize, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject obj = CreateUIObject("Text_" + textValue, parent);
        Text text = obj.AddComponent<Text>();
        text.text = textValue;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
    }

    private void CreateButton(Transform parent, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObject = CreateUIObject("Button_" + label, parent);
        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.16f, 0.16f, 0.16f, 1f);

        Button button = buttonObject.AddComponent<Button>();
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.16f, 0.16f, 0.16f, 1f);
        colors.highlightedColor = new Color(0.32f, 0.32f, 0.32f, 1f);
        colors.pressedColor = new Color(0.08f, 0.08f, 0.08f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = new Vector2(420f, 64f);

        GameObject textObject = CreateUIObject("Label", buttonObject.transform);
        Text text = textObject.AddComponent<Text>();
        text.text = label;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 26;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        StretchToFullScreen(textRect);
    }

    private void OnDisable()
    {
        if (isPaused)
        {
            Time.timeScale = 1f;
            RestorePlayerControls();
        }
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        RestorePlayerControls();
    }
}
