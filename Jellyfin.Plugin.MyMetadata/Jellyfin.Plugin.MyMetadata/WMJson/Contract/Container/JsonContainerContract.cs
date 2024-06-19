namespace WMJson {
    using System;
    using System.Text;
    internal abstract class JsonContainerContract : IJsonContract {
        private Func<object> createFunc;
        public Type Type { get; private set; }
        public JsonContainerContract(Type type) {
            Type = type;
        }
        public object Create() => (createFunc ?? (createFunc = JsonUtils.GetCreateFunc(Type)))();
        public object Parse(string value) {
            throw new NotImplementedException();
        }
        public abstract void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting);
    }
}
