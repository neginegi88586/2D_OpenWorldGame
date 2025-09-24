#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class EventSystemConverter {
    [MenuItem("ストーリー/既存シーンのEventSystemを新InputSystem対応に変換")]
    public static void Convert() {
        var eventSystems = Object.FindObjectsOfType<UnityEngine.EventSystems.EventSystem>();
        int converted = 0;
        foreach (var es in eventSystems) {
#if ENABLE_INPUT_SYSTEM
            var old = es.GetComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            if (old != null) {
                Object.DestroyImmediate(old, true);
                if (es.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>() == null) {
                    es.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
                }
                converted++;
            }
#else
            // 新InputSystem無効時は何もしない
#endif
        }
        if (converted > 0) Debug.Log($"EventSystem 変換完了: {converted} 件");
        else Debug.Log("変換対象なし (既に新InputSystem対応済みか、旧モジュール未検出)");
    }
}
#endif
