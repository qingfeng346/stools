namespace WMJson {
    using System;
    using System.Text;
    using System.Collections;
    internal class JsonEnumerableContract : JsonContainerContract {
        public IJsonContract valueContract { get; private set; }
        public JsonEnumerableContract(Type type, Type valueType) : base(type) {
            valueContract = JsonUtils.GetContract(valueType);
        }
        public override void ToJson(object obj, StringBuilder builder, JsonSerializeSetting setting) {
            var enumerable = (IEnumerable)obj;
            builder.Append('[');
            var first = true;
            foreach (var value in enumerable) {
                if (first)
                    first = false;
                else
                    builder.Append(',');
                JsonUtils.GetContract(value?.GetType()).ToJson(value, builder, setting);
            }
            builder.Append(']');
        }
    }
}
