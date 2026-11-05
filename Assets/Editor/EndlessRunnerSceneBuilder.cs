using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class EndlessRunnerSceneBuilder
{
    private const string MaterialFolder = "Assets/Art/Materials";
    private const string PrefabFolder = "Assets/Prefabs";
    private const string SceneFolder = "Assets/Scenes";
    private const string MainScenePath = "Assets/Scenes/MainScene.unity";

    [MenuItem("Tools/Endless Runner/Build Complete Demo Scene")]
    public static void BuildCompleteDemoScene()
    {
        EnsureProjectFolders();
        EnsureTags("Player", "Obstacle", "Coin");

        Material roadMaterial = GetOrCreateMaterial("Road_Asphalt", new Color(0.10f, 0.12f, 0.13f));
        Material laneMaterial = GetOrCreateMaterial("Lane_Marking", new Color(0.95f, 0.92f, 0.78f));
        Material playerMaterial = GetOrCreateMaterial("Player_Teal", new Color(0.05f, 0.77f, 0.70f));
        Material coinMaterial = GetOrCreateMaterial("Coin_Gold", new Color(1.0f, 0.75f, 0.12f));
        Material barrierMaterial = GetOrCreateMaterial("Obstacle_Red", new Color(0.90f, 0.20f, 0.18f));
        Material boxMaterial = GetOrCreateMaterial("Obstacle_Box", new Color(0.62f, 0.39f, 0.19f));
        Material wallMaterial = GetOrCreateMaterial("Obstacle_Wall", new Color(0.38f, 0.40f, 0.44f));
        Material trunkMaterial = GetOrCreateMaterial("Tree_Trunk", new Color(0.34f, 0.20f, 0.10f));
        Material leafMaterial = GetOrCreateMaterial("Tree_Leaves", new Color(0.13f, 0.55f, 0.22f));
        Material rockMaterial = GetOrCreateMaterial("Rock_Gray", new Color(0.43f, 0.45f, 0.42f));

        GameObject roadPrefab = CreateRoadPrefab(roadMaterial, laneMaterial);
        GameObject coinPrefab = CreateCoinPrefab(coinMaterial);
        GameObject barrierPrefab = CreateObstaclePrefab("Barrier", new Vector3(2.2f, 1.2f, 0.5f), barrierMaterial);
        GameObject boxPrefab = CreateObstaclePrefab("Box", new Vector3(1.4f, 1.4f, 1.4f), boxMaterial);
        GameObject wallPrefab = CreateObstaclePrefab("Wall", new Vector3(3f, 2.5f, 0.7f), wallMaterial);
        GameObject treePrefab = CreateTreePrefab(trunkMaterial, leafMaterial);
        GameObject rockPrefab = CreateRockPrefab(rockMaterial);

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject managers = new GameObject("Managers");
        GameManager gameManager = new GameObject("GameManager").AddComponent<GameManager>();
        gameManager.transform.SetParent(managers.transform);

        AudioManager audioManager = new GameObject("AudioManager").AddComponent<AudioManager>();
        audioManager.transform.SetParent(managers.transform);

        ObjectPool objectPool = new GameObject("ObjectPool").AddComponent<ObjectPool>();
        objectPool.transform.SetParent(managers.transform);
        ConfigureObjectPool(objectPool, new[]
        {
            new PoolEntry("RoadTile", roadPrefab, 8),
            new PoolEntry("Coin", coinPrefab, 30),
            new PoolEntry("Barrier", barrierPrefab, 10),
            new PoolEntry("Box", boxPrefab, 10),
            new PoolEntry("Wall", wallPrefab, 8),
            new PoolEntry("Tree", treePrefab, 12),
            new PoolEntry("Rock", rockPrefab, 12)
        });

        PlayerController player = CreatePlayer(playerMaterial);
        FollowCamera followCamera = CreateCamera(player.transform);

        SpawnManager spawnManager = new GameObject("SpawnManager").AddComponent<SpawnManager>();
        spawnManager.transform.SetParent(managers.transform);
        ConfigureSpawnManager(spawnManager, player.transform, objectPool);

        UIManager uiManager = CreateInterface();
        uiManager.transform.SetParent(managers.transform);

        CreateLighting();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene, MainScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Endless Runner demo scene built at Assets/Scenes/MainScene.unity. Press Play to test.");
    }

    private static void EnsureProjectFolders()
    {
        Directory.CreateDirectory(MaterialFolder);
        Directory.CreateDirectory(PrefabFolder);
        Directory.CreateDirectory(SceneFolder);
    }

    private static void EnsureTags(params string[] tags)
    {
        Object[] tagManagerAssets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (tagManagerAssets.Length == 0)
        {
            return;
        }

        SerializedObject tagManager = new SerializedObject(tagManagerAssets[0]);
        SerializedProperty tagsProperty = tagManager.FindProperty("tags");

        foreach (string tag in tags)
        {
            if (HasTag(tagsProperty, tag))
            {
                continue;
            }

            tagsProperty.InsertArrayElementAtIndex(tagsProperty.arraySize);
            tagsProperty.GetArrayElementAtIndex(tagsProperty.arraySize - 1).stringValue = tag;
        }

        tagManager.ApplyModifiedProperties();
    }

    private static bool HasTag(SerializedProperty tagsProperty, string tag)
    {
        for (int i = 0; i < tagsProperty.arraySize; i++)
        {
            if (tagsProperty.GetArrayElementAtIndex(i).stringValue == tag)
            {
                return true;
            }
        }

        return false;
    }

    private static Material GetOrCreateMaterial(string materialName, Color color)
    {
        string path = $"{MaterialFolder}/{materialName}.mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null)
        {
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        material = new Material(shader)
        {
            name = materialName,
            color = color
        };

        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static GameObject CreateRoadPrefab(Material roadMaterial, Material laneMaterial)
    {
        GameObject root = new GameObject("RoadTile");

        GameObject road = CreatePrimitiveChild(PrimitiveType.Cube, root.transform, "RoadSurface", roadMaterial);
        road.transform.localPosition = new Vector3(0f, -0.1f, 0f);
        road.transform.localScale = new Vector3(10f, 0.2f, 30f);

        CreateLaneMark(root.transform, -1.5f, laneMaterial);
        CreateLaneMark(root.transform, 1.5f, laneMaterial);

        return SavePrefab(root, "RoadTile");
    }

    private static void CreateLaneMark(Transform parent, float xPosition, Material laneMaterial)
    {
        GameObject lane = CreatePrimitiveChild(PrimitiveType.Cube, parent, "LaneMark", laneMaterial);
        lane.transform.localPosition = new Vector3(xPosition, 0.02f, 0f);
        lane.transform.localScale = new Vector3(0.08f, 0.02f, 28f);

        Collider laneCollider = lane.GetComponent<Collider>();
        if (laneCollider != null)
        {
            Object.DestroyImmediate(laneCollider);
        }
    }

    private static GameObject CreateCoinPrefab(Material coinMaterial)
    {
        GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        coin.name = "Coin";
        coin.tag = "Coin";
        coin.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        coin.transform.localScale = new Vector3(0.6f, 0.08f, 0.6f);
        ApplyMaterial(coin, coinMaterial);

        Collider coinCollider = coin.GetComponent<Collider>();
        coinCollider.isTrigger = true;

        coin.AddComponent<CoinSystem>();
        return SavePrefab(coin, "Coin");
    }

    private static GameObject CreateObstaclePrefab(string prefabName, Vector3 size, Material material)
    {
        GameObject root = new GameObject(prefabName);
        root.tag = "Obstacle";

        BoxCollider collider = root.AddComponent<BoxCollider>();
        collider.size = size;
        collider.center = new Vector3(0f, size.y * 0.5f, 0f);
        root.AddComponent<Obstacle>();

        GameObject visual = CreatePrimitiveChild(PrimitiveType.Cube, root.transform, $"{prefabName}_Visual", material);
        visual.tag = "Obstacle";
        visual.transform.localPosition = new Vector3(0f, size.y * 0.5f, 0f);
        visual.transform.localScale = size;

        Collider visualCollider = visual.GetComponent<Collider>();
        if (visualCollider != null)
        {
            Object.DestroyImmediate(visualCollider);
        }

        return SavePrefab(root, prefabName);
    }

    private static GameObject CreateTreePrefab(Material trunkMaterial, Material leafMaterial)
    {
        GameObject root = new GameObject("Tree");

        GameObject trunk = CreatePrimitiveChild(PrimitiveType.Cylinder, root.transform, "Trunk", trunkMaterial);
        trunk.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        trunk.transform.localScale = new Vector3(0.28f, 0.75f, 0.28f);

        GameObject leaves = CreatePrimitiveChild(PrimitiveType.Sphere, root.transform, "Leaves", leafMaterial);
        leaves.transform.localPosition = new Vector3(0f, 2f, 0f);
        leaves.transform.localScale = new Vector3(1.4f, 1.2f, 1.4f);

        RemoveCollider(trunk);
        RemoveCollider(leaves);
        return SavePrefab(root, "Tree");
    }

    private static GameObject CreateRockPrefab(Material rockMaterial)
    {
        GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rock.name = "Rock";
        rock.transform.localScale = new Vector3(1.2f, 0.65f, 0.9f);
        rock.transform.rotation = Quaternion.Euler(0f, 24f, 8f);
        ApplyMaterial(rock, rockMaterial);
        RemoveCollider(rock);

        return SavePrefab(rock, "Rock");
    }

    private static PlayerController CreatePlayer(Material playerMaterial)
    {
        GameObject playerRoot = new GameObject("Player");
        playerRoot.tag = "Player";
        playerRoot.transform.position = Vector3.zero;

        CharacterController controller = playerRoot.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.35f;
        controller.center = new Vector3(0f, 1f, 0f);

        PlayerController playerController = playerRoot.AddComponent<PlayerController>();

        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.name = "Player_Visual";
        visual.transform.SetParent(playerRoot.transform, false);
        visual.transform.localPosition = new Vector3(0f, 1f, 0f);
        visual.transform.localScale = new Vector3(0.72f, 1f, 0.72f);
        ApplyMaterial(visual, playerMaterial);
        RemoveCollider(visual);

        return playerController;
    }

    private static FollowCamera CreateCamera(Transform player)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        Camera camera = cameraObject.AddComponent<Camera>();
        cameraObject.AddComponent<AudioListener>();
        cameraObject.tag = "MainCamera";
        camera.fieldOfView = 62f;
        cameraObject.transform.position = new Vector3(0f, 5.5f, -8f);
        cameraObject.transform.LookAt(player.position + Vector3.up * 1.5f + Vector3.forward * 7f);

        FollowCamera followCamera = cameraObject.AddComponent<FollowCamera>();
        SerializedObject serializedCamera = new SerializedObject(followCamera);
        serializedCamera.FindProperty("target").objectReferenceValue = player;
        serializedCamera.ApplyModifiedPropertiesWithoutUndo();

        return followCamera;
    }

    private static void CreateLighting()
    {
        GameObject lightObject = new GameObject("Directional Light");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static UIManager CreateInterface()
    {
        GameObject canvasObject = new GameObject("Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject mainMenuPanel = CreatePanel(canvasObject.transform, "MainMenuPanel", new Color(0.03f, 0.04f, 0.05f, 0.92f));
        CreateText(mainMenuPanel.transform, "TitleText", "3D Endless Runner", 70f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.64f), new Vector2(900f, 120f));
        Button startButton = CreateButton(mainMenuPanel.transform, "StartButton", "START GAME", new Vector2(0.5f, 0.46f), new Vector2(340f, 82f));
        Button mainQuitButton = CreateButton(mainMenuPanel.transform, "QuitButton", "QUIT", new Vector2(0.5f, 0.34f), new Vector2(260f, 72f));

        GameObject hudPanel = CreatePanel(canvasObject.transform, "HUDPanel", new Color(0f, 0f, 0f, 0f));
        TMP_Text scoreText = CreateText(hudPanel.transform, "ScoreText", "Score: 0", 32f, TextAlignmentOptions.Left, new Vector2(0.02f, 0.96f), new Vector2(360f, 60f), new Vector2(0f, 1f));
        TMP_Text highScoreText = CreateText(hudPanel.transform, "HighScoreText", "High Score: 0", 32f, TextAlignmentOptions.Left, new Vector2(0.02f, 0.90f), new Vector2(440f, 60f), new Vector2(0f, 1f));
        TMP_Text coinText = CreateText(hudPanel.transform, "CoinText", "Coins: 0", 32f, TextAlignmentOptions.Left, new Vector2(0.02f, 0.84f), new Vector2(300f, 60f), new Vector2(0f, 1f));
        Button pauseButton = CreateButton(hudPanel.transform, "PauseButton", "PAUSE", new Vector2(0.93f, 0.94f), new Vector2(210f, 64f));

        GameObject pausePanel = CreatePanel(canvasObject.transform, "PausePanel", new Color(0.02f, 0.02f, 0.02f, 0.78f));
        CreateText(pausePanel.transform, "PauseTitleText", "Paused", 62f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.62f), new Vector2(680f, 110f));
        Button resumeButton = CreateButton(pausePanel.transform, "ResumeButton", "RESUME", new Vector2(0.5f, 0.47f), new Vector2(300f, 76f));
        Button pauseRestartButton = CreateButton(pausePanel.transform, "RestartButton", "RESTART", new Vector2(0.5f, 0.36f), new Vector2(300f, 76f));
        Button pauseQuitButton = CreateButton(pausePanel.transform, "QuitButton", "QUIT", new Vector2(0.5f, 0.25f), new Vector2(260f, 70f));

        GameObject gameOverPanel = CreatePanel(canvasObject.transform, "GameOverPanel", new Color(0.08f, 0.02f, 0.02f, 0.88f));
        CreateText(gameOverPanel.transform, "GameOverTitleText", "Game Over", 70f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.68f), new Vector2(760f, 115f));
        TMP_Text finalScoreText = CreateText(gameOverPanel.transform, "FinalScoreText", "Score: 0", 38f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.56f), new Vector2(520f, 70f));
        TMP_Text finalHighScoreText = CreateText(gameOverPanel.transform, "FinalHighScoreText", "High Score: 0", 38f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.49f), new Vector2(620f, 70f));
        Button gameOverRestartButton = CreateButton(gameOverPanel.transform, "RestartButton", "RESTART", new Vector2(0.5f, 0.36f), new Vector2(320f, 76f));
        Button gameOverQuitButton = CreateButton(gameOverPanel.transform, "QuitButton", "QUIT", new Vector2(0.5f, 0.25f), new Vector2(260f, 70f));

        UIManager uiManager = new GameObject("UIManager").AddComponent<UIManager>();
        ConfigureUiManager(
            uiManager,
            mainMenuPanel,
            hudPanel,
            pausePanel,
            gameOverPanel,
            scoreText,
            highScoreText,
            coinText,
            finalScoreText,
            finalHighScoreText,
            startButton,
            pauseButton,
            resumeButton,
            new[] { pauseRestartButton, gameOverRestartButton },
            new[] { mainQuitButton, pauseQuitButton, gameOverQuitButton }
        );

        CreateEventSystem();
        return uiManager;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image image = panel.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = color.a > 0.01f;

        return panel;
    }

    private static TMP_Text CreateText(
        Transform parent,
        string name,
        string text,
        float size,
        TextAlignmentOptions alignment,
        Vector2 anchor,
        Vector2 dimensions,
        Vector2? pivotOverride = null)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = pivotOverride ?? new Vector2(0.5f, 0.5f);
        rect.sizeDelta = dimensions;
        rect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI tmp = textObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = false;

        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchor, Vector2 dimensions)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = anchor;
        rect.anchorMax = anchor;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = dimensions;
        rect.anchoredPosition = Vector2.zero;

        Image image = buttonObject.AddComponent<Image>();
        image.color = new Color(0.08f, 0.68f, 0.60f, 1f);

        Button button = buttonObject.AddComponent<Button>();

        TMP_Text text = CreateText(buttonObject.transform, "Label", label, 30f, TextAlignmentOptions.Center, new Vector2(0.5f, 0.5f), dimensions);
        text.color = Color.white;

        return button;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null)
        {
            return;
        }

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
    }

    private static GameObject CreatePrimitiveChild(PrimitiveType primitiveType, Transform parent, string name, Material material)
    {
        GameObject primitive = GameObject.CreatePrimitive(primitiveType);
        primitive.name = name;
        primitive.transform.SetParent(parent, false);
        ApplyMaterial(primitive, material);
        return primitive;
    }

    private static void ApplyMaterial(GameObject target, Material material)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private static void RemoveCollider(GameObject target)
    {
        Collider collider = target.GetComponent<Collider>();
        if (collider != null)
        {
            Object.DestroyImmediate(collider);
        }
    }

    private static GameObject SavePrefab(GameObject root, string prefabName)
    {
        string path = $"{PrefabFolder}/{prefabName}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
        return prefab;
    }

    private static void ConfigureObjectPool(ObjectPool objectPool, PoolEntry[] entries)
    {
        SerializedObject serializedPool = new SerializedObject(objectPool);
        SerializedProperty poolConfigs = serializedPool.FindProperty("poolConfigs");
        poolConfigs.arraySize = entries.Length;

        for (int i = 0; i < entries.Length; i++)
        {
            SerializedProperty element = poolConfigs.GetArrayElementAtIndex(i);
            element.FindPropertyRelative("id").stringValue = entries[i].Id;
            element.FindPropertyRelative("prefab").objectReferenceValue = entries[i].Prefab;
            element.FindPropertyRelative("initialSize").intValue = entries[i].InitialSize;
            element.FindPropertyRelative("expandable").boolValue = true;
        }

        serializedPool.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureSpawnManager(SpawnManager spawnManager, Transform player, ObjectPool objectPool)
    {
        SerializedObject serializedSpawnManager = new SerializedObject(spawnManager);
        serializedSpawnManager.FindProperty("player").objectReferenceValue = player;
        serializedSpawnManager.FindProperty("objectPool").objectReferenceValue = objectPool;
        serializedSpawnManager.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void ConfigureUiManager(
        UIManager uiManager,
        GameObject mainMenuPanel,
        GameObject hudPanel,
        GameObject pausePanel,
        GameObject gameOverPanel,
        TMP_Text scoreText,
        TMP_Text highScoreText,
        TMP_Text coinText,
        TMP_Text finalScoreText,
        TMP_Text finalHighScoreText,
        Button startButton,
        Button pauseButton,
        Button resumeButton,
        Button[] restartButtons,
        Button[] quitButtons)
    {
        SerializedObject serializedUi = new SerializedObject(uiManager);
        serializedUi.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
        serializedUi.FindProperty("hudPanel").objectReferenceValue = hudPanel;
        serializedUi.FindProperty("pausePanel").objectReferenceValue = pausePanel;
        serializedUi.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        serializedUi.FindProperty("scoreText").objectReferenceValue = scoreText;
        serializedUi.FindProperty("highScoreText").objectReferenceValue = highScoreText;
        serializedUi.FindProperty("coinText").objectReferenceValue = coinText;
        serializedUi.FindProperty("finalScoreText").objectReferenceValue = finalScoreText;
        serializedUi.FindProperty("finalHighScoreText").objectReferenceValue = finalHighScoreText;
        serializedUi.FindProperty("startButton").objectReferenceValue = startButton;
        serializedUi.FindProperty("pauseButton").objectReferenceValue = pauseButton;
        serializedUi.FindProperty("resumeButton").objectReferenceValue = resumeButton;

        SetButtonArray(serializedUi.FindProperty("restartButtons"), restartButtons);
        SetButtonArray(serializedUi.FindProperty("quitButtons"), quitButtons);
        serializedUi.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetButtonArray(SerializedProperty property, Button[] buttons)
    {
        property.arraySize = buttons.Length;
        for (int i = 0; i < buttons.Length; i++)
        {
            property.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];
        }
    }

    private readonly struct PoolEntry
    {
        public readonly string Id;
        public readonly GameObject Prefab;
        public readonly int InitialSize;

        public PoolEntry(string id, GameObject prefab, int initialSize)
        {
            Id = id;
            Prefab = prefab;
            InitialSize = initialSize;
        }
    }
}
