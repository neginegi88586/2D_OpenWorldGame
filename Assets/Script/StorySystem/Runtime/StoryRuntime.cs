using System.Collections.Generic;
using System.Linq;

namespace StorySystem {
    public class StoryRuntime {
        public StoryGraph graph;
        public StoryNode current;
        public VariableStore vars = new();

        public void Start(StoryGraph g) {
            graph = g;
            current = g.startNode;
        }

        public IEnumerable<Choice> GetAvailableChoices() {
            if (current == null) yield break;
            foreach (var c in current.choices) {
                if (c == null) continue;
                if (c.conditions == null || c.conditions.Count == 0 || c.conditions.All(cond => cond == null || cond.Evaluate(vars)))
                    yield return c;
            }
        }

        public bool Choose(Choice c) {
            if (c == null) return false;
            if (!GetAvailableChoices().Contains(c)) return false;
            if (c.actions != null) {
                foreach (var a in c.actions) a?.Apply(vars);
            }
            current = c.next;
            return true;
        }

        public SaveData CreateSave() => new SaveData {
            nodeId = current != null ? current.id : null,
            vars = vars.ToSerializable()
        };

        public void Load(SaveData data) {
            if (data == null || graph == null) return;
            if (!string.IsNullOrEmpty(data.nodeId)) current = graph.GetNodeById(data.nodeId);
            vars.FromSerializable(data.vars);
        }

        [System.Serializable]
        public class SaveData {
            public string nodeId;
            public VariableStore.SerializableState vars;
        }
    }
}
