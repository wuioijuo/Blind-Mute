using UnityEngine;
using UnityEngine.UI;

public class BackroomsLevelBuilder : MonoBehaviour
{
    [Header("Build settings")]
    public bool buildOnStart = true;
    public bool cleanBeforeBuild = true;
    public int requiredItems = 5;

    [Header("Level settings")]
    public float cellSize = 3f;
    public float wallHeight = 3.1f;
    public float floorThickness = 0.1f;

    private const string GeneratedRootName = "BlindMuteGenerated_Backrooms";

    private readonly string[] map =
    {
        "############################",
        "#S....#.........#..........#",
        "#.###.#.#######.#.#######..#",
        "#...#.#.....#...#.....#..N.#",
        "###.#.#####.#.#####.#.######",
        "#...#.....#.#.....#.#......#",
        "#.#######.#.#####.#.####.#.#",
        "#..I....#.#.....#.#....#.#.#",
        "#.#####.#.#####.#.####.#.#.#",
        "#.#...#.#.....#.#....#.#...#",
        "#.#.#.#.#####.#.####.#.#####",
        "#...#.#.....#.#....#.#.....#",
        "#####.#####.#.####.#.#####.#",
        "#...I.....#.#....#.#.....#.#",
        "#.#######.#.####.#.#####.#.#",
        "#.#.....#.#....#.#.....#.#.#",
        "#.#.###.#.####.#.#####.#.#.#",
        "#.#...#.#....#.#.....#.#...#",
        "#.###.#.####.#.#####.#.###.#",
        "#...#.#....I.#.....#.#...#.#",
        "###.#.###########.#.###.#.##",
        "#...#.....N.......#.....#.##",
        "#.###########.###########.##",
        "#.....I.....#.....I.....BDE#",
        "############################"
    };

    private Material wallMaterial;
    private Material floorMaterial;
    private Material ceilingMaterial;
    private Material itemMaterial;
    private Material doorMaterial;
    private Material buttonMaterial;
    private Material noteMaterial;
    private Material exitMaterial;
    private Material lightMaterial;

    private Transform root;
    private Vector3 startPosition;
    private DoorController createdDoor;
    private UIManager createdUI;

    private int Width => map[0].Length;
    private int Height => map.Length;

    private void Start()
    {
        if (buildOnStart)
        {
            BuildLevel();
        }
    }

    [ContextMenu("Build Backrooms Level")]
    public void BuildLevel()
    {
        if (cleanBeforeBuild)
        {
            CleanGeneratedLevel();
        }

        LoadMaterials();
        GameObject rootObject = new GameObject(GeneratedRootName);
        root = rootObject.transform;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.025f;
        RenderSettings.fogColor = new Color(0.08f, 0.075f, 0.045f);
        RenderSettings.ambientLight = new Color(0.12f, 0.11f, 0.07f);

        CreateLevelGeometry();
        createdUI = CreateUI();
        createdDoor = null;
        CreateGameplayObjects();
        CreatePlayer();
        CreateGameManager();
        CreateAudio();
    }

    [ContextMenu("Clean Generated Level")]
    public void CleanGeneratedLevel()
    {
        DestroyByName(GeneratedRootName);
        DestroyByName("Player");
        DestroyByName("LevelCanvas");
        DestroyByName("GameManager");
        DestroyByName("Main Camera");
        DestroyByName("EventSystem");
        DestroyByName("Backrooms_Audio");
    }

    private void DestroyByName(string objectName)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        for (int i = allObjects.Length - 1; i >= 0; i--)
        {
            if (allObjects[i] != null && allObjects[i].name == objectName)
            {
                DestroyObjectSmart(allObjects[i]);
            }
        }
    }

    private void DestroyObjectSmart(Object obj)
    {
        if (obj == null) return;

        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
            DestroyImmediate(obj);
        }
    }

    private void LoadMaterials()
    {
        wallMaterial = LoadMaterial("Backrooms/Materials/wall1", new Color(0.73f, 0.66f, 0.38f));
        floorMaterial = LoadMaterial("Backrooms/Materials/carpet1", new Color(0.50f, 0.42f, 0.26f));
        ceilingMaterial = CreateMaterial("Ceiling_Yellow", new Color(0.68f, 0.63f, 0.42f));
        itemMaterial = CreateMaterial("Echo_Item", new Color(0.05f, 0.8f, 1f));
        doorMaterial = CreateMaterial("Exit_Door", new Color(0.32f, 0.34f, 0.31f));
        buttonMaterial = CreateMaterial("Button_Red", new Color(0.9f, 0.1f, 0.05f));
        noteMaterial = CreateMaterial("Note_White", new Color(0.95f, 0.9f, 0.68f));
        exitMaterial = CreateMaterial("Exit_Green", new Color(0.1f, 1f, 0.45f));
        lightMaterial = CreateMaterial("Fluorescent_Light", new Color(1f, 0.92f, 0.62f));
        lightMaterial.EnableKeyword("_EMISSION");
        lightMaterial.SetColor("_EmissionColor", new Color(1f, 0.86f, 0.45f) * 1.2f);
    }

    private Material LoadMaterial(string path, Color fallbackColor)
    {
        Material loaded = Resources.Load<Material>(path);
        if (loaded != null)
        {
            return loaded;
        }

        return CreateMaterial("Fallback_" + path.Replace('/', '_'), fallbackColor);
    }

    private Material CreateMaterial(string name, Color color)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.name = name;
        material.color = color;
        return material;
    }

    private void CreateLevelGeometry()
    {
        Transform geometryRoot = new GameObject("Geometry").transform;
        geometryRoot.SetParent(root);

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char c = map[y][x];
                Vector3 position = CellToWorld(x, y);

                if (c == '#')
                {
                    CreateCube("Wall", position + Vector3.up * (wallHeight / 2f), new Vector3(cellSize, wallHeight, cellSize), wallMaterial, geometryRoot);
                }
                else
                {
                    CreateCube("Floor", position + Vector3.down * (floorThickness / 2f), new Vector3(cellSize, floorThickness, cellSize), floorMaterial, geometryRoot);
                    CreateCube("Ceiling", position + Vector3.up * wallHeight, new Vector3(cellSize, floorThickness, cellSize), ceilingMaterial, geometryRoot);
                }
            }
        }

        for (int y = 2; y < Height - 2; y += 4)
        {
            for (int x = 2; x < Width - 2; x += 5)
            {
                if (IsWalkableCell(x, y))
                {
                    CreateLightFixture(CellToWorld(x, y), geometryRoot);
                }
            }
        }
    }

    private void CreateGameplayObjects()
    {
        Transform gameplayRoot = new GameObject("GameplayObjects").transform;
        gameplayRoot.SetParent(root);

        // First pass: find start and create the door before the button receives a reference to it.
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char c = map[y][x];
                Vector3 position = CellToWorld(x, y);

                if (c == 'S')
                {
                    startPosition = position + new Vector3(0f, 0.2f, 0f);
                }
                else if (c == 'D')
                {
                    createdDoor = CreateDoor(position, gameplayRoot);
                }
            }
        }

        int itemIndex = 1;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                char c = map[y][x];
                Vector3 position = CellToWorld(x, y);

                if (c == 'I')
                {
                    CreateCollectible(position, itemIndex, gameplayRoot);
                    itemIndex++;
                }
                else if (c == 'N')
                {
                    CreateNote(position, gameplayRoot);
                }
                else if (c == 'B')
                {
                    CreateButton(position, gameplayRoot);
                }
                else if (c == 'E')
                {
                    CreateExit(position, gameplayRoot);
                }
            }
        }
    }

    private void CreateCollectible(Vector3 position, int index, Transform parent)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        item.name = "Fragment_" + index;
        item.transform.SetParent(parent);
        item.transform.position = position + new Vector3(0f, 0.75f, 0f);
        item.transform.localScale = new Vector3(0.35f, 0.55f, 0.35f);
        item.GetComponent<Renderer>().material = itemMaterial;
        item.AddComponent<Rotator>().rotationSpeed = new Vector3(0f, 85f, 0f);

        CollectibleItem collectible = item.AddComponent<CollectibleItem>();
        collectible.itemName = "Фрагмент памяти " + index;
        item.AddComponent<EchoTarget>();

        Light light = item.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 4.5f;
        light.intensity = 0.45f;
        light.color = new Color(0.2f, 0.9f, 1f);
    }

    private void CreateNote(Vector3 position, Transform parent)
    {
        GameObject note = CreateCube("Note", position + new Vector3(0f, 1.0f, 0f), new Vector3(1.15f, 0.12f, 0.8f), noteMaterial, parent);
        note.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        NoteInteractable interactable = note.AddComponent<NoteInteractable>();
        interactable.noteText = "Записка: не доверяй прямым коридорам. Эхо показывает то, что глаза не видят.";
        note.AddComponent<EchoTarget>();
    }

    private DoorController CreateDoor(Vector3 position, Transform parent)
    {
        GameObject door = CreateCube("ExitDoor", position + Vector3.up * (wallHeight / 2f), new Vector3(cellSize * 0.92f, wallHeight, cellSize * 0.92f), doorMaterial, parent);
        DoorController controller = door.AddComponent<DoorController>();
        controller.isLocked = true;
        door.AddComponent<EchoTarget>();
        return controller;
    }

    private void CreateButton(Vector3 position, Transform parent)
    {
        GameObject pedestal = CreateCube("ButtonPedestal", position + new Vector3(0f, 0.45f, 0f), new Vector3(1.2f, 0.9f, 1.2f), wallMaterial, parent);
        pedestal.AddComponent<EchoTarget>();

        GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        button.name = "ExitButton";
        button.transform.SetParent(parent);
        button.transform.position = position + new Vector3(0f, 1.05f, 0f);
        button.transform.localScale = new Vector3(0.55f, 0.16f, 0.55f);
        button.GetComponent<Renderer>().material = buttonMaterial;

        ButtonInteractable interactable = button.AddComponent<ButtonInteractable>();
        interactable.door = createdDoor;
        button.AddComponent<EchoTarget>();

        Light light = button.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 5f;
        light.intensity = 0.7f;
        light.color = new Color(1f, 0.1f, 0.05f);
    }

    private void CreateExit(Vector3 position, Transform parent)
    {
        GameObject exit = new GameObject("ExitZone");
        exit.transform.SetParent(parent);
        exit.transform.position = position + new Vector3(0f, 1.1f, 0f);

        BoxCollider collider = exit.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.size = new Vector3(cellSize * 0.8f, 2.2f, cellSize * 0.8f);
        exit.AddComponent<ExitZone>();

        GameObject marker = CreateCube("ExitMarker", position + new Vector3(0f, 1.6f, 0f), new Vector3(1.2f, 2.5f, 0.15f), exitMaterial, parent);
        marker.AddComponent<EchoTarget>();

        Light light = marker.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 6f;
        light.intensity = 1f;
        light.color = new Color(0.2f, 1f, 0.45f);
    }

    private void CreateLightFixture(Vector3 position, Transform parent)
    {
        GameObject fixture = CreateCube("FluorescentLight", position + new Vector3(0f, wallHeight - 0.15f, 0f), new Vector3(cellSize * 0.65f, 0.06f, 0.18f), lightMaterial, parent);
        fixture.transform.rotation = Quaternion.Euler(0f, Random.value > 0.5f ? 0f : 90f, 0f);

        Light light = fixture.AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 7f;
        light.intensity = 0.75f;
        light.color = new Color(1f, 0.88f, 0.55f);
    }

    private void CreatePlayer()
    {
        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = startPosition;
        player.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.32f;
        controller.center = new Vector3(0f, 0.9f, 0f);
        controller.stepOffset = 0.35f;

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.62f, 0f);
        cameraObject.transform.localRotation = Quaternion.identity;

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.nearClipPlane = 0.03f;
        camera.farClipPlane = 24f;
        camera.fieldOfView = 68f;

        AudioListener listener = cameraObject.AddComponent<AudioListener>();
        listener.enabled = true;

        Light headLight = cameraObject.AddComponent<Light>();
        headLight.type = LightType.Spot;
        headLight.range = 12f;
        headLight.spotAngle = 52f;
        headLight.intensity = 1.0f;
        headLight.color = new Color(1f, 0.92f, 0.68f);

        PlayerController movement = player.AddComponent<PlayerController>();
        movement.cameraTransform = cameraObject.transform;

        PlayerInteraction interaction = player.AddComponent<PlayerInteraction>();
        interaction.playerCamera = camera;
        interaction.uiManager = createdUI;

        EchoAbility echo = player.AddComponent<EchoAbility>();
        echo.uiManager = createdUI;

        PlayerFallReset fallReset = player.AddComponent<PlayerFallReset>();
        fallReset.safePosition = startPosition;
        fallReset.minY = -2f;
    }

    private void CreateGameManager()
    {
        GameObject manager = new GameObject("GameManager");
        GameManager gameManager = manager.AddComponent<GameManager>();
        gameManager.requiredItems = requiredItems;
        gameManager.uiManager = createdUI;
        gameManager.exitDoor = createdDoor;
    }

    private UIManager CreateUI()
    {
        GameObject canvasObject = new GameObject("LevelCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObject.AddComponent<GraphicRaycaster>();

        UIManager ui = canvasObject.AddComponent<UIManager>();

        ui.itemCounterText = CreateText(canvasObject.transform, "ItemCounter", "Фрагменты: 0/" + requiredItems, 24, TextAnchor.UpperLeft, new Color(0.9f, 0.88f, 0.62f));
        SetRect(ui.itemCounterText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(25f, -25f), new Vector2(500f, 50f));

        ui.objectiveText = CreateText(canvasObject.transform, "Objective", "Найди 5 фрагментов", 22, TextAnchor.UpperLeft, new Color(0.75f, 0.75f, 0.65f));
        SetRect(ui.objectiveText.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(25f, -62f), new Vector2(750f, 50f));

        ui.hintText = CreateText(canvasObject.transform, "Hint", "", 28, TextAnchor.MiddleCenter, Color.white);
        SetRect(ui.hintText.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 150f), new Vector2(900f, 55f));
        ui.hintText.gameObject.SetActive(false);

        ui.messageText = CreateText(canvasObject.transform, "Message", "", 28, TextAnchor.MiddleCenter, new Color(0.9f, 0.9f, 0.7f));
        SetRect(ui.messageText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 250f), new Vector2(1000f, 85f));
        ui.messageText.gameObject.SetActive(false);

        Text controls = CreateText(canvasObject.transform, "Controls", "WASD — движение  |  E — действие  |  SPACE — эхо  |  ESC — курсор", 18, TextAnchor.LowerCenter, new Color(0.7f, 0.7f, 0.65f));
        SetRect(controls.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 20f), new Vector2(900f, 35f));

        CreateVignette(canvasObject.transform);
        return ui;
    }

    private void CreateVignette(Transform canvas)
    {
        CreateEdgePanel(canvas, "VignetteTop", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(0f, 160f));
        CreateEdgePanel(canvas, "VignetteBottom", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 160f));
        CreateEdgePanel(canvas, "VignetteLeft", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(170f, 0f));
        CreateEdgePanel(canvas, "VignetteRight", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(0f, 0f), new Vector2(170f, 0f));
    }

    private void CreateEdgePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0f, 0f, 0f, 0.42f);
        image.raycastTarget = false;
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private Text CreateText(Transform parent, string name, string text, int fontSize, TextAnchor alignment, Color color)
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

    private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(anchorMin.x, anchorMin.y);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;
    }

    private void CreateAudio()
    {
        AudioClip hum = Resources.Load<AudioClip>("Backrooms/Sounds/60hz hum");
        if (hum == null) return;

        GameObject audioObject = new GameObject("Backrooms_Audio");
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = hum;
        source.loop = true;
        source.volume = 0.28f;
        source.spatialBlend = 0f;
        source.Play();
    }

    private GameObject CreateCube(string name, Vector3 position, Vector3 scale, Material material, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.localScale = scale;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.material = material;
        }

        return obj;
    }

    private Vector3 CellToWorld(int x, int y)
    {
        float worldX = (x - Width / 2f) * cellSize + cellSize / 2f;
        float worldZ = (Height / 2f - y) * cellSize - cellSize / 2f;
        return new Vector3(worldX, 0f, worldZ);
    }

    private bool IsWalkableCell(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return false;
        return map[y][x] != '#';
    }
}
