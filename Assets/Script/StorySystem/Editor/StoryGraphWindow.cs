#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using StorySystem;
using System.Linq;

public class StoryGraphWindow : EditorWindow {
    private StoryGraph graph;
    private Vector2 nodeListScroll;
    private Vector2 nodeDetailScroll;
    private StoryNode selected;

    [MenuItem("ストーリー/ストーリー編集ウィンドウ")] public static void Open() {
        GetWindow<StoryGraphWindow>("ストーリー編集");
    }

    void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        DrawLeft();
        DrawRight();
        EditorGUILayout.EndHorizontal();
    }

    void DrawLeft() {
        EditorGUILayout.BeginVertical(GUILayout.Width(260));
    graph = (StoryGraph)EditorGUILayout.ObjectField("ストーリー全体アセット", graph, typeof(StoryGraph), false);
    if (graph == null) { EditorGUILayout.HelpBox("ストーリー全体(StoryGraph) を指定してください", MessageType.Info); EditorGUILayout.EndVertical(); return; }

            if (GUILayout.Button("会話ノード追加")) {
            CreateNode();
        }
        if (GUILayout.Button("ID整列/重複チェック")) {
            ValidateIds();
        }

    EditorGUILayout.LabelField("会話ノード一覧", EditorStyles.boldLabel);
        nodeListScroll = EditorGUILayout.BeginScrollView(nodeListScroll, GUILayout.ExpandHeight(true));
        foreach (var n in graph.nodes.Where(n=>n!=null)) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(selected == n, n.id == string.Empty ? "<no id>" : n.id, "Button")) {
                selected = n;
            }
            if (GUILayout.Button("×", GUILayout.Width(24))) {
                if (EditorUtility.DisplayDialog("削除", n.id + " を削除します", "OK", "Cancel")) {
                    string path = AssetDatabase.GetAssetPath(n);
                    graph.nodes.Remove(n);
                    if (selected == n) selected = null;
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.SaveAssets();
                    break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void DrawRight() {
        EditorGUILayout.BeginVertical();
        if (graph == null) { EditorGUILayout.EndVertical(); return; }
        if (graph.startNode == null && graph.nodes.Count > 0) {
            if (GUILayout.Button("最初の会話ノードを Start に設定")) graph.startNode = graph.nodes[0];
        } else {
            graph.startNode = (StoryNode)EditorGUILayout.ObjectField("StartNode", graph.startNode, typeof(StoryNode), false);
        }

        if (selected == null) { EditorGUILayout.HelpBox("ノードを選択してください", MessageType.Info); EditorGUILayout.EndVertical(); return; }

        nodeDetailScroll = EditorGUILayout.BeginScrollView(nodeDetailScroll);
        EditorGUI.BeginChangeCheck();
    selected.id = EditorGUILayout.TextField("ノードID", selected.id);
        EditorGUILayout.LabelField("本文");
        selected.bodyKey = EditorGUILayout.TextArea(selected.bodyKey, GUILayout.MinHeight(60));

        EditorGUILayout.Space();
    EditorGUILayout.LabelField("選択肢一覧", EditorStyles.boldLabel);
        var choices = selected.choices;
        for (int i=0; i<choices.Count; i++) {
            var c = choices[i];
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            c.textKey = EditorGUILayout.TextField("表示テキスト(キー/本文)", c.textKey);
            if (GUILayout.Button("削除", GUILayout.Width(48))) { choices.RemoveAt(i); break; }
            EditorGUILayout.EndHorizontal();
            c.next = (StoryNode)EditorGUILayout.ObjectField("遷移先ノード", c.next, typeof(StoryNode), false);

            // 条件
            EditorGUILayout.LabelField("条件", EditorStyles.miniBoldLabel);
            for (int ci=0; ci<c.conditions.Count; ci++) {
                EditorGUILayout.BeginHorizontal();
                c.conditions[ci] = (ConditionAsset)EditorGUILayout.ObjectField(c.conditions[ci], typeof(ConditionAsset), false);
                if (GUILayout.Button("-", GUILayout.Width(24))) { c.conditions.RemoveAt(ci); break; }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("条件追加")) c.conditions.Add(null);

            // アクション
            EditorGUILayout.LabelField("アクション", EditorStyles.miniBoldLabel);
            for (int ai=0; ai<c.actions.Count; ai++) {
                EditorGUILayout.BeginHorizontal();
                c.actions[ai] = (StoryActionAsset)EditorGUILayout.ObjectField(c.actions[ai], typeof(StoryActionAsset), false);
                if (GUILayout.Button("-", GUILayout.Width(24))) { c.actions.RemoveAt(ai); break; }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("アクション追加")) c.actions.Add(null);

            EditorGUILayout.EndVertical();
        }
        if (GUILayout.Button("選択肢追加")) choices.Add(new StorySystem.Choice());

        if (EditorGUI.EndChangeCheck()) {
            EditorUtility.SetDirty(selected);
            EditorUtility.SetDirty(graph);
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    void CreateNode() {
        string path = EditorUtility.SaveFilePanelInProject("StoryNode作成", "NewStoryNode", "asset", "場所を選択");
        if (string.IsNullOrEmpty(path)) return;
        var node = ScriptableObject.CreateInstance<StoryNode>();
        node.id = System.IO.Path.GetFileNameWithoutExtension(path);
        AssetDatabase.CreateAsset(node, path);
        AssetDatabase.SaveAssets();
        graph.nodes.Add(node);
        if (graph.startNode == null) graph.startNode = node;
        selected = node;
    }

    void ValidateIds() {
        var duplicates = graph.nodes.GroupBy(n=>n.id).Where(g=>g.Key!="" && g.Count()>1).Select(g=>g.Key).ToList();
        if (duplicates.Count == 0) EditorUtility.DisplayDialog("チェック", "重複IDなし", "OK");
        else EditorUtility.DisplayDialog("重複あり", string.Join(", ", duplicates), "OK");
    }
}
#endif
