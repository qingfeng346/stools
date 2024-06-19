namespace WMJson {
    using System;
    using System.Text;
    internal class JsonArrayContract : JsonContainerContract {
        public IJsonContract valueContract { get; private set; }
        public Type elementType { get; private set; }
        public int Rank { get; private set; }
        public JsonArrayContract(Type type, Type valueType) : base(type) {
            valueContract = JsonUtils.GetContract(valueType);
            elementType = valueType;
            Rank = type.GetArrayRank();
        }
        public override void ToJson(object obj, StringBuilder builder, JsonSerializeSetting setting) {
            if (Rank > 1) {
                var array = (Array)obj;
                ToJson(array, new int[array.Rank], 0, builder, setting);
            } else {
                var array = (Array)obj;
                builder.Append('[');
                var first = true;
                for (var i = 0; i < array.Length; ++i) {
                    if (i != 0) builder.Append(",");
                    var value = array.GetValue(i);
                    JsonUtils.GetContract(value?.GetType()).ToJson(value, builder, setting);
                }
                builder.Append(']');
            }
        }
        void ToJson(Array array, int[] indices, int dimension, StringBuilder builder, JsonSerializeSetting setting) {
            if (dimension == array.Rank) {
                // 所有维度的索引都已经确定，打印当前元素
                var value = array.GetValue(indices);
                JsonUtils.GetContract(value?.GetType()).ToJson(value, builder, setting);
                return;
            }
            builder.Append("[");
            for (int i = 0; i < array.GetLength(dimension); i++) {
                if (i != 0) builder.Append(",");
                // 设置当前维度的索引
                indices[dimension] = i;
                ToJson(array, indices, dimension + 1, builder, setting);
            }
            builder.Append("]");
        }
    }
}
