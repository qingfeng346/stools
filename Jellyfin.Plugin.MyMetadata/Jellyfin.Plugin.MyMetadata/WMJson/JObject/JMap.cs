namespace WMJson {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    public class JMap : JObject, IDictionary<string, JObject> {
        private Dictionary<string, JObject> values = new Dictionary<string, JObject>();
        public JMap() : base(JObjectType.Map) {
            Value = this;
        }
        public override JObject this[string key] { 
            get => values[key]; 
            set => values[key] = value;
        }
        public virtual ICollection<string> Keys => values.Keys;
        public ICollection<JObject> Values => values.Values;
        public int Count => values.Count;
        public bool IsReadOnly => true;
        public void Add(string key, JObject value) => values.Add(key, value);
        public void Add(KeyValuePair<string, JObject> item) => values.Add(item.Key, item.Value);
        public void Clear() => values.Clear();
        public bool Contains(KeyValuePair<string, JObject> item) => throw new NotImplementedException();
        public bool ContainsKey(string key) => values.ContainsKey(key);
        public IEnumerator<KeyValuePair<string, JObject>> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
        public bool TryGetValue(string key, out JObject value) => values.TryGetValue(key, out value);
        public bool Remove(string key) => values.Remove(key);
        public bool Remove(KeyValuePair<string, JObject> item) => values.Remove(item.Key);
        public void CopyTo(KeyValuePair<string, JObject>[] array, int arrayIndex) => throw new NotImplementedException();

        public static explicit operator Dictionary<string, JObject>(JMap v) => v.values;

        public static explicit operator JMap(Dictionary<string, JObject> v) => new JMap() { values = v };
    }
}
