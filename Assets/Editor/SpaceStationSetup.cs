using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class SpaceStationSetup
{
    private const string SCENES_PATH = "Assets/Scenes";
    private const string SPRITES_PATH = "Assets/Sprites";
    private const string WHITE_PNG = "Assets/Sprites/White.png";

    // Space colour palette
    private static readonly Color BG_COLOR     = new Color(0.039f, 0.039f, 0.059f);
    private static readonly Color WALL_COLOR   = new Color(0.102f, 0.165f, 0.290f);
    private static readonly Color PLAYER_COLOR = new Color(0.10f,  0.90f,  1.00f);
    private static readonly Color ENEMY1_COLOR = new Color(1.00f,  0.20f,  0.10f);
    private static readonly Color ENEMY2_COLOR = new Color(1.00f,  0.55f,  0.05f);
    private static readonly Color ENEMY3_COLOR = new Color(0.90f,  0.10f,  0.80f);
    private static readonly Color UI_COLOR     = new Color(0.00f,  1.00f,  0.80f);

    private static Sprite whiteSprite;

    // ─────────────────────────────────────────────────────────────────
    [MenuItem("Tools/Space Station Survivor/Setup All Scenes")]
    public static void SetupAllScenes()
    {
        Debug.Log("[SSS] Starting full scene setup…");
        EnsureFolders();
        EnsureTags("Coletavel", "Enemy");
        EnsureWhiteSprite();
        ConfigureWebGL();
        SetupMainMenu();
        SetupGame();
        SetupGameOver();
        ConfigureBuildSettings();
        EditorSceneManager.OpenScene($"{SCENES_PATH}/Game.unity");
        Debug.Log("[SSS] Setup complete! Open Tools > Space Station Survivor to rerun.");
    }

    // ─────────────────────────────────────────────────────────────────
    // Folders & assets
    // ─────────────────────────────────────────────────────────────────
    private static void EnsureFolders()
    {
        string[] paths = {
            "Assets/Scenes", "Assets/Sprites", "Assets/Prefabs",
            "Assets/Scripts", "Assets/Scripts/Core",
            "Assets/Scripts/Player", "Assets/Scripts/Enemy",
            "Assets/Scripts/Audio", "Assets/Scripts/UI", "Assets/Editor"
        };
        foreach (var p in paths)
        {
            var parts = p.Split('/');
            string cur = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = cur + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(cur, parts[i]);
                cur = next;
            }
        }
        AssetDatabase.Refresh();
    }

    private static void EnsureTags(params string[] tags)
    {
        var tm = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var arr = tm.FindProperty("tags");
        foreach (var tag in tags)
        {
            bool found = false;
            for (int i = 0; i < arr.arraySize; i++)
                if (arr.GetArrayElementAtIndex(i).stringValue == tag) { found = true; break; }
            if (found) continue;
            arr.InsertArrayElementAtIndex(arr.arraySize);
            arr.GetArrayElementAtIndex(arr.arraySize - 1).stringValue = tag;
            Debug.Log($"[SSS] Tag added: {tag}");
        }
        tm.ApplyModifiedProperties();
    }

    private static void EnsureWhiteSprite()
    {
        Directory.CreateDirectory(Application.dataPath + "/Sprites");
        string full = Application.dataPath + "/Sprites/White.png";
        if (!File.Exists(full))
        {
            var tex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            tex.SetPixels(Enumerable.Repeat(Color.white, 64 * 64).ToArray());
            tex.Apply();
            File.WriteAllBytes(full, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
        }
        AssetDatabase.Refresh();
        var imp = (TextureImporter)AssetImporter.GetAtPath(WHITE_PNG);
        if (imp != null)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.spritePixelsPerUnit = 64;
            imp.filterMode = FilterMode.Point;
            imp.SaveAndReimport();
        }
        whiteSprite = AssetDatabase.LoadAssetAtPath<Sprite>(WHITE_PNG);
    }

    private static void ConfigureWebGL()
    {
        PlayerSettings.defaultWebScreenWidth  = 900;
        PlayerSettings.defaultWebScreenHeight = 600;
        PlayerSettings.runInBackground = true;
    }

    // ─────────────────────────────────────────────────────────────────
    // Main Menu Scene
    // ─────────────────────────────────────────────────────────────────
    private static void SetupMainMenu()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SetupCamera(BG_COLOR);

        var canvas = MakeCanvas("Canvas", 900, 600);
        var mmUI   = canvas.AddComponent<MainMenuUI>();

        // Dark background
        MakePanel(canvas.transform, "BG", V2(0,0), V2(1,1), V2(0,0), V2(0,0),
                  new Color(0.02f, 0.02f, 0.05f));

        // Title
        MakeTMP(canvas.transform, "Title", "SPACE STATION SURVIVOR",
                V2(0.5f,1), V2(0.5f,1), V2(0,-90), V2(720,100),
                52, UI_COLOR, FontStyles.Bold, TextAlignmentOptions.Center);

        // Subtitle
        MakeTMP(canvas.transform, "Subtitle", "Colete todos os cristais antes que o O2 acabe!",
                V2(0.5f,1), V2(0.5f,1), V2(0,-170), V2(700,35),
                19, new Color(0.7f,0.7f,0.9f), FontStyles.Normal, TextAlignmentOptions.Center);

        // Controls hint
        MakeTMP(canvas.transform, "Controls",
                "WASD / Setas / Analógico esquerdo  ·  R = Reiniciar",
                V2(0.5f,0), V2(0.5f,0), V2(0,50), V2(700,28),
                14, new Color(0.4f,0.4f,0.6f), FontStyles.Normal, TextAlignmentOptions.Center);

        // Start button
        var startBtn = MakeButton(canvas.transform, "StartBtn", "INICIAR MISSAO",
                       V2(0.5f,0.5f), V2(0,-20), V2(280,62),
                       new Color(0,0.65f,0.9f), Color.white, 22);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            startBtn.GetComponent<Button>().onClick, mmUI.OnStartButton);

        MakeEventSystem();
        EditorSceneManager.SaveScene(scene, $"{SCENES_PATH}/MainMenu.unity");
        Debug.Log("[SSS] MainMenu saved");
    }

    // ─────────────────────────────────────────────────────────────────
    // Game Scene
    // ─────────────────────────────────────────────────────────────────
    private static void SetupGame()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SetupCamera(BG_COLOR);

        // Managers
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("AudioManager").AddComponent<AudioManager>();

        MakePlayer();
        MakeWalls();
        MakeEnemies();
        MakeCrystals();
        MakeHUD();
        MakeEventSystem();

        EditorSceneManager.SaveScene(scene, $"{SCENES_PATH}/Game.unity");
        Debug.Log("[SSS] Game scene saved");
    }

    private static void MakePlayer()
    {
        var go = new GameObject("Player");
        go.tag = "Player";
        go.transform.position = Vector3.zero;
        go.transform.localScale = V3(0.5f, 0.5f, 1);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = whiteSprite;
        sr.color  = PLAYER_COLOR;
        sr.sortingOrder = 5;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        var col = go.AddComponent<CircleCollider2D>();
        col.radius = 0.45f;

        go.AddComponent<PlayerController>();

        var pi = go.AddComponent<PlayerInput>();
        var ia = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        if (ia != null) { pi.actions = ia; pi.defaultActionMap = "Player"; }
        pi.notificationBehavior = PlayerNotifications.SendMessages;

        go.AddComponent<AudioSource>().playOnAwake = false;
    }

    private static void MakeWalls()
    {
        // Ortho size 5, aspect 900/600 = 1.5 → viewport 15 × 10 units
        var parent = new GameObject("Walls").transform;
        MakeWall(parent, "WallTop",    V3( 0.0f,  4.8f, 0), V3(16f, 0.4f, 1));
        MakeWall(parent, "WallBottom", V3( 0.0f, -4.8f, 0), V3(16f, 0.4f, 1));
        MakeWall(parent, "WallLeft",   V3(-7.6f,  0.0f, 0), V3(0.4f, 10f, 1));
        MakeWall(parent, "WallRight",  V3( 7.6f,  0.0f, 0), V3(0.4f, 10f, 1));
    }

    private static void MakeWall(Transform parent, string name, Vector3 pos, Vector3 scale)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;
        go.transform.localScale = scale;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = whiteSprite;
        sr.color  = WALL_COLOR;
        sr.sortingOrder = 1;

        go.AddComponent<BoxCollider2D>();
    }

    private static void MakeEnemies()
    {
        var parent = new GameObject("Enemies").transform;

        var cfgs = new (Color col, Vector3 pos, Vector2 ptA, Vector2 ptB, float spd)[]
        {
            (ENEMY1_COLOR, V3( 2f,  2f, 0), V2(-5f, 2f),    V2(5f,   2f),    2.0f),
            (ENEMY2_COLOR, V3(-3f, -1f, 0), V2(-3f, -3.5f), V2(-3f,  1.5f), 1.7f),
            (ENEMY3_COLOR, V3( 4f, -3f, 0), V2( 1f, -3f),   V2(6.5f, -3f),  2.2f),
        };

        for (int i = 0; i < cfgs.Length; i++)
        {
            var c  = cfgs[i];
            var go = new GameObject($"Enemy{i + 1}");
            go.transform.SetParent(parent);
            go.transform.position   = c.pos;
            go.transform.localScale = V3(0.55f, 0.55f, 1);
            go.tag = "Enemy";

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = whiteSprite;
            sr.color  = c.col;
            sr.sortingOrder = 4;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            go.AddComponent<BoxCollider2D>();

            var ep = go.AddComponent<EnemyPatrol>();
            var so = new SerializedObject(ep);
            so.FindProperty("pointA").vector2Value = c.ptA;
            so.FindProperty("pointB").vector2Value = c.ptB;
            so.FindProperty("speed").floatValue    = c.spd;
            so.ApplyModifiedProperties();
        }
    }

    private static void MakeCrystals()
    {
        var parent = new GameObject("Crystals").transform;

        var positions = new Vector3[]
        {
            V3(-5.5f,  3.5f, 0), V3(-2.5f,  3.0f, 0), V3( 1.0f,  3.5f, 0),
            V3( 5.0f,  3.0f, 0), V3( 6.0f,  0.0f, 0), V3(-6.0f,  0.5f, 0),
            V3(-4.0f, -2.5f, 0), V3( 0.5f, -3.5f, 0), V3( 3.5f, -1.5f, 0),
            V3(-1.5f,  1.0f, 0),
        };

        var colors = new Color[]
        {
            new Color(0.2f, 1.0f, 0.4f),
            new Color(0.3f, 0.8f, 1.0f),
            new Color(1.0f, 0.9f, 0.2f),
        };

        for (int i = 0; i < positions.Length; i++)
        {
            var go = new GameObject($"Crystal{i + 1}");
            go.transform.SetParent(parent);
            go.transform.position   = positions[i];
            go.transform.localScale = V3(0.35f, 0.35f, 1);
            go.tag = "Coletavel";

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = whiteSprite;
            sr.color  = colors[i % colors.Length];
            sr.sortingOrder = 3;

            go.AddComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    private static void MakeHUD()
    {
        var canvas = MakeCanvas("Canvas_HUD", 900, 600);
        var uiCtrl = canvas.AddComponent<UIController>();

        // O2 — top left
        var o2 = MakeTMP(canvas.transform, "O2Text", "O2: 60s",
            V2(0,1), V2(0,1), V2(115,-20), V2(220,36),
            24, UI_COLOR, FontStyles.Bold, TextAlignmentOptions.Center);

        // Score — below O2
        var score = MakeTMP(canvas.transform, "ScoreText", "Score: 0",
            V2(0,1), V2(0,1), V2(110,-55), V2(210,30),
            19, new Color(0.9f,0.9f,1), FontStyles.Normal, TextAlignmentOptions.Center);

        // Lives — top right
        var lives = MakeTMP(canvas.transform, "LivesText", "♥ ♥ ♥",
            V2(1,1), V2(1,1), V2(-90,-20), V2(170,36),
            24, new Color(1,0.3f,0.3f), FontStyles.Bold, TextAlignmentOptions.Center);

        // Crystals — below lives
        var crystals = MakeTMP(canvas.transform, "CrystalsText", "Cristais: 0/10",
            V2(1,1), V2(1,1), V2(-95,-55), V2(180,30),
            16, new Color(0.4f,1,0.5f), FontStyles.Normal, TextAlignmentOptions.Center);

        // Wire up serialized fields
        var so = new SerializedObject(uiCtrl);
        so.FindProperty("o2Text").objectReferenceValue       = o2.GetComponent<TextMeshProUGUI>();
        so.FindProperty("scoreText").objectReferenceValue    = score.GetComponent<TextMeshProUGUI>();
        so.FindProperty("livesText").objectReferenceValue    = lives.GetComponent<TextMeshProUGUI>();
        so.FindProperty("crystalsText").objectReferenceValue = crystals.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();
    }

    // ─────────────────────────────────────────────────────────────────
    // Game Over Scene
    // ─────────────────────────────────────────────────────────────────
    private static void SetupGameOver()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SetupCamera(BG_COLOR);

        var canvas = MakeCanvas("Canvas", 900, 600);
        var goUI   = canvas.AddComponent<GameOverUI>();

        // Dark overlay
        MakePanel(canvas.transform, "BG", V2(0,0), V2(1,1), V2(0.5f,0.5f), V2(0,0),
                  new Color(0.02f, 0.02f, 0.06f, 0.97f));

        // Result (big)
        var result = MakeTMP(canvas.transform, "ResultText", "RESULTADO",
            V2(0.5f,1), V2(0.5f,1), V2(0,-115), V2(700,80),
            48, Color.white, FontStyles.Bold, TextAlignmentOptions.Center);

        // Stats
        var scoreGO = MakeTMP(canvas.transform, "ScoreText", "Score Final: 0",
            V2(0.5f,1), V2(0.5f,1), V2(0,-215), V2(500,48),
            28, new Color(0.9f,0.9f,1), FontStyles.Normal, TextAlignmentOptions.Center);

        var timeGO = MakeTMP(canvas.transform, "TimeText", "O2 Restante: 0s",
            V2(0.5f,1), V2(0.5f,1), V2(0,-270), V2(500,40),
            24, UI_COLOR, FontStyles.Normal, TextAlignmentOptions.Center);

        var crystalGO = MakeTMP(canvas.transform, "CrystalsText", "Cristais: 0/10",
            V2(0.5f,1), V2(0.5f,1), V2(0,-318), V2(500,40),
            24, new Color(0.4f,1,0.5f), FontStyles.Normal, TextAlignmentOptions.Center);

        // Buttons
        var restartBtn = MakeButton(canvas.transform, "RestartBtn", "REINICIAR",
            V2(0.5f,0.5f), V2(-120,-90), V2(210,58),
            new Color(0,0.65f,0.9f), Color.white, 20);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            restartBtn.GetComponent<Button>().onClick, goUI.OnRestartButton);

        var menuBtn = MakeButton(canvas.transform, "MenuBtn", "MENU PRINCIPAL",
            V2(0.5f,0.5f), V2(120,-90), V2(210,58),
            new Color(0.25f,0.25f,0.45f), Color.white, 18);
        UnityEditor.Events.UnityEventTools.AddPersistentListener(
            menuBtn.GetComponent<Button>().onClick, goUI.OnMainMenuButton);

        // Hint
        MakeTMP(canvas.transform, "HintText",
            "Pressione R durante o jogo para reiniciar a qualquer momento",
            V2(0.5f,0), V2(0.5f,0), V2(0,35), V2(650,26),
            13, new Color(0.35f,0.35f,0.55f), FontStyles.Normal, TextAlignmentOptions.Center);

        // Wire up GameOverUI
        var so = new SerializedObject(goUI);
        so.FindProperty("resultText").objectReferenceValue   = result.GetComponent<TextMeshProUGUI>();
        so.FindProperty("scoreText").objectReferenceValue    = scoreGO.GetComponent<TextMeshProUGUI>();
        so.FindProperty("timeText").objectReferenceValue     = timeGO.GetComponent<TextMeshProUGUI>();
        so.FindProperty("crystalsText").objectReferenceValue = crystalGO.GetComponent<TextMeshProUGUI>();
        so.ApplyModifiedProperties();

        MakeEventSystem();
        EditorSceneManager.SaveScene(scene, $"{SCENES_PATH}/GameOver.unity");
        Debug.Log("[SSS] GameOver scene saved");
    }

    // ─────────────────────────────────────────────────────────────────
    // Build settings
    // ─────────────────────────────────────────────────────────────────
    private static void ConfigureBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene($"{SCENES_PATH}/MainMenu.unity", true),
            new EditorBuildSettingsScene($"{SCENES_PATH}/Game.unity",     true),
            new EditorBuildSettingsScene($"{SCENES_PATH}/GameOver.unity", true),
        };
        Debug.Log("[SSS] Build settings: 3 scenes registered (MainMenu=0, Game=1, GameOver=2)");
    }

    // ─────────────────────────────────────────────────────────────────
    // UI helpers
    // ─────────────────────────────────────────────────────────────────
    private static void SetupCamera(Color bg)
    {
        var go  = new GameObject("Main Camera");
        var cam = go.AddComponent<Camera>();
        cam.orthographic      = true;
        cam.orthographicSize  = 5f;
        cam.clearFlags        = CameraClearFlags.SolidColor;
        cam.backgroundColor   = bg;
        go.transform.position = V3(0, 0, -10);
        go.tag = "MainCamera";
    }

    private static GameObject MakeCanvas(string name, float w, float h)
    {
        var go     = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(w, h);
        scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight  = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    private static GameObject MakePanel(Transform parent, string name,
        Vector2 ancMin, Vector2 ancMax, Vector2 pivot,
        Vector2 ancPos, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = ancMin; rt.anchorMax = ancMax;
        rt.pivot = pivot; rt.anchoredPosition = ancPos;
        rt.sizeDelta = Vector2.zero;
        return go;
    }

    private static GameObject MakeTMP(Transform parent, string name, string text,
        Vector2 ancMin, Vector2 ancMax, Vector2 ancPos, Vector2 size,
        float fontSize, Color color, FontStyles style, TextAlignmentOptions align)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text       = text;
        tmp.fontSize   = fontSize;
        tmp.color      = color;
        tmp.fontStyle  = style;
        tmp.alignment  = align;
        tmp.enableWordWrapping = false;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = ancMin; rt.anchorMax = ancMax;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = ancPos;
        rt.sizeDelta = size;
        return go;
    }

    private static GameObject MakeButton(Transform parent, string name, string label,
        Vector2 anchor, Vector2 ancPos, Vector2 size, Color bg, Color fg, float fontSize)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = bg;
        var btn  = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var cols = btn.colors;
        cols.highlightedColor = bg * 1.3f; cols.pressedColor = bg * 0.7f;
        btn.colors = cols;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor; rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = ancPos; rt.sizeDelta = size;

        var lbl = new GameObject("Label");
        lbl.transform.SetParent(go.transform, false);
        var tmp = lbl.AddComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.fontSize = fontSize;
        tmp.color = fg; tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        var lrt = lbl.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;
        return go;
    }

    private static void MakeEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<InputSystemUIInputModule>();
    }

    // ─────────────────────────────────────────────────────────────────
    // Shorthand helpers
    // ─────────────────────────────────────────────────────────────────
    private static Vector2 V2(float x, float y) => new Vector2(x, y);
    private static Vector3 V3(float x, float y, float z) => new Vector3(x, y, z);
}
