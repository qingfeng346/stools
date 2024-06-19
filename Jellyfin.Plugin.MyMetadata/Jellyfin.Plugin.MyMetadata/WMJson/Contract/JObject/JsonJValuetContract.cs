
using System.Text;
namespace WMJson {
    internal class JsonJValuetContract : JsonJObjectContract {
        public override void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            var jvalue = (JValue)value;
            switch (jvalue.ValueType) {
                case JObjectType.Null: builder.Append("null"); break;
                case JObjectType.True: builder.Append("true"); break;
                case JObjectType.False: builder.Append("false"); break;
                case JObjectType.String: JsonUtils.StringContract.ToJson(jvalue.Value, builder, setting); break;
                default: builder.Append(jvalue.Value); break;
            }
        }
    }
}
