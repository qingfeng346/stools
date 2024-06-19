namespace WMJson {
    using System;
    using System.Text;
    using System.Collections;
    internal class JsonMapContract : JsonContainerContract {
        public IJsonContract keyContract { get; private set; }
        public IJsonContract valueContract { get; private set; }
        public JsonMapContract(Type type, Type keyType, Type valueType) : base(type) {
            keyContract = JsonUtils.GetContract(keyType);
            valueContract = JsonUtils.GetContract(valueType);
        }
        public override void ToJson(object obj, StringBuilder builder, JsonSerializeSetting setting) {
            var enumerator = ((IDictionary)obj).GetEnumerator();
            builder.Append('{');
            var first = true;
            while (enumerator.MoveNext()) {
                if (first)
                    first = false;
                else
                    builder.Append(',');
                JsonUtils.StringContract.ToJson(enumerator.Key, builder, setting);
                builder.Append(":");
                var value = enumerator.Value;
                JsonUtils.GetContract(value?.GetType()).ToJson(value, builder, setting);
            }
            builder.Append('}');
        }
    }
}
