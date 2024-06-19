using System.Reflection;
using System.Text;
using System;
using System.Collections.Generic;
namespace WMJson {
    internal class JsonObjectContract : JsonContainerContract {
        private JsonVariable[] jsonVariables;
        public JsonObjectContract(Type type) : base(type) {
            var list = new List<JsonVariable>();
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
                if (!JsonUtils.IsAttribute<JsonIgnoreAttribute>(propertyInfo) && propertyInfo.GetIndexParameters().Length == 0)
                    list.Add(new JsonProperty(propertyInfo));
            }
            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.Instance)) {
                if (!JsonUtils.IsAttribute<JsonIgnoreAttribute>(fieldInfo))
                    list.Add(new JsonField(fieldInfo));
            }
            jsonVariables = list.ToArray();
        }
        public bool TryGetVariable(string key, out JsonVariable variable) {
            for (var i = 0; i < jsonVariables.Length; ++i) {
                if (jsonVariables[i].Name == key) {
                    variable = jsonVariables[i];
                    return true;
                }
            }
            variable = null;
            return false;
        }
        public override void ToJson(object obj, StringBuilder builder, JsonSerializeSetting setting) {
            builder.Append('{');
            var first = true;
            for (var i = 0; i < jsonVariables.Length; ++i) {
                var variable = jsonVariables[i];
                if (!variable.CanRead) continue;
                var value = variable.GetValue(obj);
                if (value == null && setting.IgnoreNull) continue;
                if (first)
                    first = false;
                else
                    builder.Append(',');
                JsonUtils.StringContract.ToJson(variable.Name, builder, setting);
                builder.Append(":");
                JsonUtils.GetContract(value?.GetType()).ToJson(value, builder, setting);
            }
            builder.Append('}');
        }
    }
}
