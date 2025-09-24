using System.Collections.Generic;
using UnityEngine;

namespace StorySystem {
    [System.Serializable]
    public class Choice {
        public string textKey; // ローカライズキー or 直接文
        public StoryNode next;
        public List<ConditionAsset> conditions = new List<ConditionAsset>();
        public List<StoryActionAsset> actions = new List<StoryActionAsset>();
    }

    [CreateAssetMenu(menuName = "ストーリー/ストーリー会話ノード")]
    public class StoryNode : ScriptableObject {
        public string id; // ユニーク ID
        [TextArea] public string bodyKey; // 本文(キー)
        public List<Choice> choices = new List<Choice>();
    }
}
