namespace WMJson {
    using System.Collections;
    using System.Collections.Generic;
    public class JArray : JObject, IList<JObject> {
        private List<JObject> values = new List<JObject>();
        public JArray() : base(JObjectType.Array) {
            Value = this;
        }
        public JObject this[int index] { 
            get => values[index]; 
            set => values[index] = value;
        }
        public int Count => values.Count;
        public bool IsReadOnly => false;
        public override void Add(JObject obj) => values.Add(obj);
        public void Clear() => values.Clear();
        public bool Contains(JObject item) => values.Contains(item);
        public void CopyTo(JObject[] array, int arrayIndex) => values.CopyTo(array, arrayIndex);
        public IEnumerator<JObject> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => values.GetEnumerator();
        public int IndexOf(JObject item) => values.IndexOf(item);
        public void Insert(int index, JObject item) => values.Insert(index, item);
        public bool Remove(JObject item) => values.Remove(item);
        public void RemoveAt(int index) => values.RemoveAt(index);

        public static explicit operator List<JObject>(JArray v) => v.values;

        public static explicit operator JArray(List<JObject> v) => new JArray() { values = v };
    }
}
