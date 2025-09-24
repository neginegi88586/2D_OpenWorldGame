#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using StorySystem;
using System.IO;

public static class StoryCreateMenu {
    private const string DefaultFolder = "Assets/Story";

    [MenuItem("ストーリー/新規ストーリー全体アセット作成")]
    public static void CreateStoryGraph() {
        EnsureFolder();
        var graph = ScriptableObject.CreateInstance<StoryGraph>();
        string path = GetUniquePath("StoryGraph.asset");
        AssetDatabase.CreateAsset(graph, path);
        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(graph);
    }

    [MenuItem("ストーリー/ストーリーに会話ノード追加")]
    public static void CreateStoryNode() {
        EnsureFolder();
        var node = ScriptableObject.CreateInstance<StoryNode>();
        node.id = "Node" + System.Guid.NewGuid().ToString("N").Substring(0,6);
        string path = GetUniquePath(node.id + ".asset");
        AssetDatabase.CreateAsset(node, path);
        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(node);
    }

    private static void EnsureFolder() {
        if (!AssetDatabase.IsValidFolder(DefaultFolder)) {
            AssetDatabase.CreateFolder("Assets", "Story");
            AssetDatabase.CreateFolder(DefaultFolder, "Nodes");
            AssetDatabase.SaveAssets();
        }
    }

    private static string GetUniquePath(string fileName) {
        string basePath = DefaultFolder;
        if (fileName.Contains("StoryNode")) basePath = Path.Combine(DefaultFolder, "Nodes");
        if (!AssetDatabase.IsValidFolder(Path.Combine(DefaultFolder, "Nodes"))) {
            AssetDatabase.CreateFolder(DefaultFolder, "Nodes");
        }
        string path = Path.Combine(basePath, fileName).Replace("\\","/");
        return AssetDatabase.GenerateUniqueAssetPath(path);
    }
}
#endif
