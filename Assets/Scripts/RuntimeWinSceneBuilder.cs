using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RuntimeWinSceneBuilder : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        BuildWinScreen();
    }

    private void BuildWinScreen()
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
        cam.backgroundColor = new Color(0.015f, 0.03f, 0.025f);

        GameObject canvasObject = CreateCanvas("WinCanvas");

        GameObject background = CreatePanel(canvasObject.transform, "Background", new Color(0.015f, 0.03f, 0.025f, 1f));
        Stretch(background.GetComponent<RectTransform>());

        Text title = CreateText(canvasObject.transform, "Title", "УРОВЕНЬ ПРОЙДЕН", 58, TextAnchor.MiddleCenter, new Color(0.78f, 1f, 0.78f));
        SetRect(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 95), new Vector2(900, 90));

        Text subtitle = CreateText(canvasObject.transform, "Subtitle", "Ты выбрался из первой комнаты", 26, TextAnchor.MiddleCenter, new Color(0.75f, 0.85f, 0.75f));
        SetRect(subtitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(900, 50));

        Button menuButton = CreateButton(canvasObject.transform, "MenuButton", "В ГЛАВНОЕ МЕНЮ", new Vector2(0, -80));
        menuButton.onClick.AddListener(() => SceneManager.LoadScene(SceneNames.MainMenu));
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
        image.color = new Color(0.55f, 0.85f, 0.55f, 0.95f);
        Button button = obj.AddComponent<Button>();
        SetRect(obj.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(380, 58));

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
