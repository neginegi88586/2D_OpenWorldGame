#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using System.IO;
using StorySystem;

public static class StorySampleCreator {
    private const string RootFolder = "Assets/StorySample";

    [MenuItem("ストーリー/サンプルシーンとストーリー作成", priority = 50)]
    public static void CreateSample() {
        if (AssetDatabase.IsValidFolder(RootFolder)) {
            if (!EditorUtility.DisplayDialog("サンプル再生成","既存の StorySample を再生成しますか?","再生成","キャンセル")) return;
        }
        EnsureFolders();
        var graph = CreateGraph();
        CreateNodes(graph);
        EditorUtility.SetDirty(graph);
        AssetDatabase.SaveAssets();
        var scene = CreateScene(graph);
        var scenePath = AssetDatabase.GetAssetPath(scene);
        if (!string.IsNullOrEmpty(scenePath)) {
            EditorSceneManager.OpenScene(scenePath);
        } else {
            Debug.LogWarning("シーンのパス取得に失敗しました");
        }
        EditorGUIUtility.PingObject(graph);
        Debug.Log("サンプルストーリーとシーンを生成しました。再生ボタンで確認できます。");
    }

    private static void EnsureFolders() {
        if (!AssetDatabase.IsValidFolder(RootFolder)) AssetDatabase.CreateFolder("Assets", "StorySample");
        if (!AssetDatabase.IsValidFolder(RootFolder+"/Nodes")) AssetDatabase.CreateFolder(RootFolder, "Nodes");
        AssetDatabase.SaveAssets();
    }

    private static StoryGraph CreateGraph() {
        var graph = ScriptableObject.CreateInstance<StoryGraph>();
        AssetDatabase.CreateAsset(graph, RootFolder+"/SampleGraph.asset");
        return graph;
    }

    private static void CreateNodes(StoryGraph graph) {
        // シンプルな 3 ノード構成: Start -> 分岐(YES/NO) -> End
        var start = CreateNode("start", "ここはサンプルです。どうしますか?\n(YES/NO)");
        var yes = CreateNode("yes_path", "YES を選びました！\nこの後は自由に増やして下さい。");
        var no = CreateNode("no_path", "NO を選びました… 別ルートです。");

        // 選択肢設定
        start.choices.Add(new Choice{ textKey = "YES", next = yes });
        start.choices.Add(new Choice{ textKey = "NO", next = no });

        graph.nodes.Add(start); graph.nodes.Add(yes); graph.nodes.Add(no);
        graph.startNode = start;

        void SaveNode(StoryNode n) { EditorUtility.SetDirty(n); }
        SaveNode(start); SaveNode(yes); SaveNode(no);
    }

    private static StoryNode CreateNode(string id, string body) {
        var node = ScriptableObject.CreateInstance<StoryNode>();
        node.id = id;
        node.bodyKey = body;
        AssetDatabase.CreateAsset(node, $"{RootFolder}/Nodes/{id}.asset");
        return node;
    }

    private static SceneAsset CreateScene(StoryGraph graph) {
        // 新規シーン
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Canvas
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // EventSystem (新 Input System 用)
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null) {
            var es = new GameObject("EventSystem", typeof(UnityEngine.EventSystems.EventSystem));
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
#endif
        }

        // フォント取得 (TMP優先)
        var fontData = FindFontAssets();
        if (fontData.tmpFont == null && fontData.legacyFont == null) {
            Debug.LogWarning("GenEi / NotoSans いずれのフォントも見つかりませんでした。");
        }

    // Body Text (TMP使用)
    var bodyGO = new GameObject("BodyText", typeof(TextMeshProUGUI));
    bodyGO.transform.SetParent(canvasGO.transform, false);
    var bodyTextTMP = bodyGO.GetComponent<TextMeshProUGUI>();
    bodyTextTMP.text = "本文";
    bodyTextTMP.fontSize = 42;
    if (fontData.tmpFont != null) bodyTextTMP.font = fontData.tmpFont;
    bodyTextTMP.alignment = TextAlignmentOptions.TopLeft;
    bodyTextTMP.enableWordWrapping = true;
    var bodyRect = bodyGO.GetComponent<RectTransform>();
        bodyRect.anchorMin = new Vector2(0.05f, 0.55f);
        bodyRect.anchorMax = new Vector2(0.95f, 0.95f);
        bodyRect.offsetMin = bodyRect.offsetMax = Vector2.zero;

    // Choices Root (レイアウト用コンテナ)
    var choicesRoot = new GameObject("ChoicesRoot", typeof(UnityEngine.UI.VerticalLayoutGroup), typeof(ContentSizeFitter)).transform;
    choicesRoot.SetParent(canvasGO.transform);
    var crRect = choicesRoot.GetComponent<RectTransform>();
    crRect.anchorMin = new Vector2(0.05f, 0.1f);
    crRect.anchorMax = new Vector2(0.45f, 0.5f);
    crRect.offsetMin = crRect.offsetMax = Vector2.zero;
    var vlg = choicesRoot.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
    vlg.spacing = 12f;
    vlg.childForceExpandHeight = false;
    vlg.childForceExpandWidth = true;
    vlg.childControlHeight = true;
    vlg.childControlWidth = true;
    var fitter = choicesRoot.GetComponent<ContentSizeFitter>();
    fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

    // Button テンプレート (VerticalLayoutGroup 下で複製される)
    var btnTemplate = new GameObject("ChoiceButton", typeof(Image), typeof(Button), typeof(LayoutElement));
    btnTemplate.transform.SetParent(choicesRoot);
        var btnTextGO = new GameObject("Text", typeof(TextMeshProUGUI));
        btnTextGO.transform.SetParent(btnTemplate.transform, false);
        var btnTMP = btnTextGO.GetComponent<TextMeshProUGUI>();
        btnTMP.text = "選択肢";
        if (fontData.tmpFont != null) btnTMP.font = fontData.tmpFont;
        btnTMP.fontSize = 34;
        btnTMP.alignment = TextAlignmentOptions.Midline;
        btnTMP.color = Color.white;
        var img = btnTemplate.GetComponent<Image>();
        img.color = new Color(0.15f,0.25f,0.45f,1f); // 濃い背景
        var button = btnTemplate.GetComponent<Button>();
        var colors = button.colors;
        colors.normalColor = img.color;
        colors.highlightedColor = new Color(0.22f,0.35f,0.6f,1f);
        colors.pressedColor = new Color(0.1f,0.18f,0.32f,1f);
        colors.selectedColor = colors.highlightedColor;
        colors.disabledColor = new Color(0.15f,0.15f,0.15f,0.6f);
        button.colors = colors;
    var btnRect = btnTemplate.GetComponent<RectTransform>();
    btnRect.anchorMin = new Vector2(0f, 0f);
    btnRect.anchorMax = new Vector2(1f, 0f);
    btnRect.pivot = new Vector2(0.5f, 0.5f);
    btnRect.sizeDelta = new Vector2(0, 60f);
    var le = btnTemplate.GetComponent<LayoutElement>();
    le.minHeight = 50f;
    le.preferredHeight = 60f;
    var btnTextRect = btnTextGO.GetComponent<RectTransform>();
    btnTextRect.anchorMin = Vector2.zero; btnTextRect.anchorMax = Vector2.one; btnTextRect.offsetMin = btnTextRect.offsetMax = Vector2.zero;

        // StoryUI
        var storyUI = canvasGO.AddComponent<StoryUI>();
        storyUI.graph = graph;
    storyUI.bodyTextTMP = bodyTextTMP;
        storyUI.choicesRoot = choicesRoot;
        storyUI.choiceButtonPrefab = btnTemplate.GetComponent<Button>();

        // シーン保存
        string scenePath = RootFolder + "/SampleScene.unity";
        EditorSceneManager.SaveScene(newScene, scenePath);
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
    }

    private struct FontAssets { public TMP_FontAsset tmpFont; public Font legacyFont; }
    private static FontAssets FindFontAssets() {
        var fa = new FontAssets();
        string[] tmpCandidates = {
            "Assets/Font/GenEiAntiqueNv5-M SDF.asset",
            "Assets/Font/NotoSansJP-Regular SDF.asset"
        };
        foreach (var c in tmpCandidates) if (File.Exists(c)) { fa.tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(c); if (fa.tmpFont!=null) break; }
        if (fa.tmpFont == null) {
            string[] legacy = {
                "Assets/Font/GenEiAntiqueNv5-M.ttf",
                "Assets/Font/NotoSansJP-Regular.ttf"
            };
            foreach (var c in legacy) if (File.Exists(c)) { fa.legacyFont = AssetDatabase.LoadAssetAtPath<Font>(c); if (fa.legacyFont!=null) break; }
        }
        return fa;
    }
}
#endif
