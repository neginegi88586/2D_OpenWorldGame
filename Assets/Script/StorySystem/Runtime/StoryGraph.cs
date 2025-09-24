using System.Collections.Generic;
using UnityEngine;

namespace StorySystem {
    [CreateAssetMenu(menuName = "ストーリー/ストーリー全体(グラフ)")]
    public class StoryGraph : ScriptableObject {
        public StoryNode startNode;
        public List<StoryNode> nodes = new List<StoryNode>();

        public StoryNode GetNodeById(string id) {
            return nodes.Find(n => n != null && n.id == id);
        }
    }
}
