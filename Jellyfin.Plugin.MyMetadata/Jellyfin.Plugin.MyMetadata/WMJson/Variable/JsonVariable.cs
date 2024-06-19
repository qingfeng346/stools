namespace WMJson {
    using System;
    internal abstract class JsonVariable {
        public string Name { get; protected set; }
        public IJsonContract contract { get; protected set; }
        public abstract bool CanRead { get; }
        public abstract bool CanWrite { get; }
        public JsonVariable(string name, Type type) {
            Name = name;
            contract = JsonUtils.GetContract(type);
        }
        public abstract void SetValue(object obj, object value);
        public abstract object GetValue(object obj);
    }
}
