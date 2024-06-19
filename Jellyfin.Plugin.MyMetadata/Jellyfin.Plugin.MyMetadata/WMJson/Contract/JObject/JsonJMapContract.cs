
using System.Text;
namespace WMJson {
    internal class JsonJMapContract : JsonJObjectContract {
        public override void ToJson(object obj, StringBuilder builder, JsonSerializeSetting setting) {
            var map = (JMap)obj;
            builder.Append('{');
            var first = true;
            foreach (var pair in map) {
                if (first)
                    first = false;
                else
                    builder.Append(',');
                JsonUtils.StringContract.ToJson(pair.Key, builder, setting);
                builder.Append(":");
                var value = pair.Value.Value;
                JsonUtils.GetContract(value?.GetType()).ToJson(value, builder, setting);
            }
            builder.Append('}');
        }
    }
}
