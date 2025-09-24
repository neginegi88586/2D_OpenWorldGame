using UnityEngine;

namespace StorySystem {
    public abstract class StoryActionAsset : ScriptableObject {
        public abstract void Apply(VariableStore store);
    }

    [CreateAssetMenu(menuName = "Story/Actions/SetInt")]
    public class SetIntAction : StoryActionAsset {
        public string key; public int value;
        public override void Apply(VariableStore store) => store.SetInt(key, value);
    }

    [CreateAssetMenu(menuName = "Story/Actions/SetBool")]
    public class SetBoolAction : StoryActionAsset {
        public string key; public bool value = true;
        public override void Apply(VariableStore store) => store.SetBool(key, value);
    }
}
