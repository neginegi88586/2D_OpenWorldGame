using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using StorySystem;
using System.Collections.Generic;

public class StoryUI : MonoBehaviour {
    public StoryGraph graph;
    public Text bodyText; // レガシーUGUI互換(設定されていれば使用)
    public TextMeshProUGUI bodyTextTMP; // 推奨（こちら優先）
    public Transform choicesRoot;
    public Button choiceButtonPrefab;

    private StoryRuntime runtime = new();
    private readonly List<Button> spawned = new();

    void Start() {
        if (graph != null) {
            runtime.Start(graph);
            Refresh();
        }
    }

    public void SetGraph(StoryGraph g) { graph = g; runtime.Start(g); Refresh(); }

    void ClearButtons() {
        foreach (var b in spawned) if (b) Destroy(b.gameObject);
        spawned.Clear();
    }

    void Refresh() {
        string body = runtime.current != null ? runtime.current.bodyKey : "<END>";
        if (bodyTextTMP) bodyTextTMP.text = body;
        else if (bodyText) bodyText.text = body;
        ClearButtons();
        if (runtime.current == null) return;
        Button first = null;
        foreach (var c in runtime.GetAvailableChoices()) {
            var btn = Instantiate(choiceButtonPrefab, choicesRoot);
            spawned.Add(btn);
            // TMP or Text どちらか存在する方
            TMP_Text tmpChild = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpChild) tmpChild.text = c.textKey; else {
                var txt = btn.GetComponentInChildren<Text>();
                if (txt) {
                    txt.text = c.textKey;
                    var img = btn.GetComponent<Image>();
                    if (img != null) {
                        float luminance = 0.2126f * img.color.r + 0.7152f * img.color.g + 0.0722f * img.color.b;
                        txt.color = luminance > 0.6f ? Color.black : Color.white;
                    }
                }
            }
            btn.onClick.AddListener(()=>{ runtime.Choose(c); Refresh(); });
            var rt = btn.GetComponent<RectTransform>();
            if (rt != null) {
                rt.anchorMin = new Vector2(0, 0.5f);
                rt.anchorMax = new Vector2(1, 0.5f);
                rt.offsetMin = new Vector2(0, rt.offsetMin.y);
                rt.offsetMax = new Vector2(0, rt.offsetMax.y);
            }
            if (btn.GetComponent<UnityEngine.UI.LayoutElement>() == null) {
                var le = btn.gameObject.AddComponent<UnityEngine.UI.LayoutElement>();
                le.preferredHeight = 60f;
            }
            if (first == null) first = btn;
        }
        // 最初の選択肢にフォーカスを当てる（キーボード/ゲームパッド操作しやすく）
        if (first != null) {
            first.Select();
        }
    }

#if ENABLE_INPUT_SYSTEM
    void Update() {
        if (spawned.Count == 0) return;
        // ゲームパッド/キーボード 'Enter' / 'Space' で現在選択中ボタンを押下
        if (Keyboard.current != null) {
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame) {
                var current = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;
                if (current != null) {
                    var btn = current.GetComponent<Button>();
                    if (btn != null) btn.onClick.Invoke();
                }
            }
        }
        if (Gamepad.current != null) {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) {
                var current = UnityEngine.EventSystems.EventSystem.current?.currentSelectedGameObject;
                if (current != null) {
                    var btn = current.GetComponent<Button>();
                    if (btn != null) btn.onClick.Invoke();
                }
            }
        }
    }
#endif
}
