using UnityEngine;

namespace StorySystem {
    public abstract class ConditionAsset : ScriptableObject {
        public abstract bool Evaluate(VariableStore store);
    }

    [CreateAssetMenu(menuName = "Story/Conditions/IntCompare")]
    public class IntCompareCondition : ConditionAsset {
        public string key;
        public enum Op { GreaterEqual, LessEqual, Equal }
        public Op op;
        public int value;
        public override bool Evaluate(VariableStore store) {
            int v = store.GetInt(key);
            return op switch {
                Op.GreaterEqual => v >= value,
                Op.LessEqual => v <= value,
                Op.Equal => v == value,
                _ => false
            };
        }
    }

    [CreateAssetMenu(menuName = "Story/Conditions/BoolValue")]
    public class BoolValueCondition : ConditionAsset {
        public string key;
        public bool expected = true;
        public override bool Evaluate(VariableStore store) => store.GetBool(key) == expected;
    }
}
