using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuntimeMainMenuBuilder : MonoBehaviour
{
    public string levelSceneName = SceneNames.Level;

    private void Start()
    {
        BuildMenu();
    }

    private void BuildMenu()
    {
        EnsureEventSystem();

        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            cam = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
        }
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.02f, 0.018f, 0.012f);

        GameObject canvasObject = CreateCanvas("MainMenuCanvas");
        Canvas canvas = canvasObject.GetComponent<Canvas>();

        GameObject background = CreatePanel(canvasObject.transform, "Background", new Color(0.02f, 0.018f, 0.012f, 1f));
        Stretch(background.GetComponent<RectTransform>());

        Text title = CreateText(canvasObject.transform, "Title", "BLIND & MUTE", 58, TextAnchor.MiddleCenter, new Color(0.95f, 0.9f, 0.55f));
        SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 165), new Vector2(900, 90));

        Text subtitle = CreateText(canvasObject.transform, "Subtitle", "BLIND & MUTE — визуальный симулятор ограниченного восприятия", 23, TextAnchor.MiddleCenter, new Color(0.8f, 0.78f, 0.62f));
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 95), new Vector2(1000, 50));

        Button startButton = CreateButton(canvasObject.transform, "StartButton", "НАЧАТЬ ИГРУ", new Vector2(0, 0));
        startButton.onClick.AddListener(() => SceneManager.LoadScene(levelSceneName));

        Button exitButton = CreateButton(canvasObject.transform, "ExitButton", "ВЫХОД", new Vector2(0, -80));
        exitButton.onClick.AddListener(Application.Quit);

        Text controls = CreateText(canvasObject.transform, "Controls", "WASD — движение    E — взаимодействие    SPACE — эхо", 20, TextAnchor.MiddleCenter, new Color(0.7f, 0.7f, 0.65f));
        SetRect(controls.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0, 38), new Vector2(900, 40));
    }

    private static GameObject CreateCanvas(string name)
    {
        GameObject canvasObject = new GameObject(name);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        canvasObject.AddComponent<GraphicRaycaster>();
        return canvasObject;
    }

    private static void EnsureEventSystem()
    {
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    }

    private static Text CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Text t = obj.AddComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = fontSize;
        t.alignment = alignment;
        t.color = color;
        return t;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 position)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.72f, 0.66f, 0.35f, 0.95f);
        Button button = obj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.9f, 0.84f, 0.42f, 1f);
        colors.pressedColor = new Color(0.55f, 0.5f, 0.25f, 1f);
        button.colors = colors;
        SetRect(obj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(360, 58));

        Text text = CreateText(obj.transform, "Text", label, 25, TextAnchor.MiddleCenter, Color.black);
        Stretch(text.rectTransform);
        return button;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image image = obj.AddComponent<Image>();
        image.color = color;
        return obj;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }
}
