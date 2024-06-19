using System.Text;

namespace WMJson {
    internal class JsonJArrayContract : JsonJObjectContract {
        public override void ToJson(object obj, StringBuilder builder, JsonSerializeSetting setting) {
            var list = (JArray)obj;
            builder.Append('[');
            var first = true;
            for (var i = 0; i < list.Count; ++i) {
                if (first)
                    first = false;
                else
                    builder.Append(',');
                var value = list[i];
                JsonUtils.GetContract(value.Value?.GetType()).ToJson(value.Value, builder, setting);
            }
            builder.Append(']');
        }
    }
}
