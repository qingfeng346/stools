namespace WMJson {
    using System;
    using System.Text;
    internal class JsonEnumContract : JsonPrimitiveContract {
        private Type type;
        public JsonEnumContract(Type type) {
            this.type = type;
        }
        public override object Parse(string value) => Enum.Parse(type, value);
        public override void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            builder.Append('\"');
            builder.Append(value);
            builder.Append('\"');
        }
    }
}
