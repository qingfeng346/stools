using System.Text;

namespace WMJson {
    internal class JsonStringContract : JsonPrimitiveContract {
        public override object Parse(string value) => value;
        public override void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            builder.Append('\"');
            foreach (var c in value.ToString().ToCharArray()) {
                switch (c) {
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }
            builder.Append('\"');
        }
    }
}
