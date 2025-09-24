using System.Collections.Generic;

namespace StorySystem {
    public class VariableStore {
        private readonly Dictionary<string, int> ints = new();
        private readonly Dictionary<string, bool> bools = new();

        public int GetInt(string key, int def = 0) => ints.TryGetValue(key, out var v) ? v : def;
        public void SetInt(string key, int value) => ints[key] = value;
        public bool GetBool(string key, bool def = false) => bools.TryGetValue(key, out var v) ? v : def;
        public void SetBool(string key, bool value) => bools[key] = value;

        public SerializableState ToSerializable() => new SerializableState { ints = new Dictionary<string, int>(ints), bools = new Dictionary<string, bool>(bools) };
        public void FromSerializable(SerializableState s) {
            ints.Clear(); bools.Clear();
            if (s?.ints != null) foreach (var kv in s.ints) ints[kv.Key] = kv.Value;
            if (s?.bools != null) foreach (var kv in s.bools) bools[kv.Key] = kv.Value;
        }

        [System.Serializable]
        public class SerializableState {
            public Dictionary<string, int> ints;
            public Dictionary<string, bool> bools;
        }
    }
}
