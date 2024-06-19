using System.Text;

namespace WMJson {
    internal class JsonBoolContract : JsonPrimitiveContract {
        public override object Parse(string value) => bool.Parse(value);
        public override void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            builder.Append(true.Equals(value) ? "true" : "false");
        }
    }
}
