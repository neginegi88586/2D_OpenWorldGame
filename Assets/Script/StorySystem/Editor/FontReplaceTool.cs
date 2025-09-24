#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public static class FontReplaceTool {
    [MenuItem("ストーリー/現在シーンのUIフォントを GenEiAntique に差し替え")] 
    public static void ReplaceFontsInScene() {
        var font = LoadGenEiFont();
        if (font == null) { Debug.LogWarning("GenEiAntiqueNv5-M フォントが見つかりませんでした。"); return; }
        int count = 0;
        foreach (var text in Object.FindObjectsOfType<Text>(true)) {
            if (text.font != font) { text.font = font; count++; EditorUtility.SetDirty(text); }
        }
        Debug.Log($"フォント差し替え完了: {count} 件");
    }

    private static Font LoadGenEiFont() {
        string[] candidates = {
            "Assets/Font/GenEiAntiqueNv5-M SDF.asset",
            "Assets/Font/GenEiAntiqueNv5-M.ttf"
        };
        foreach (var c in candidates) if (File.Exists(c)) return AssetDatabase.LoadAssetAtPath<Font>(c);
        return null;
    }
}
#endif
