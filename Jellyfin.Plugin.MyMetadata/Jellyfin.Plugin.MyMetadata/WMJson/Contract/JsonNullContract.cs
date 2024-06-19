namespace WMJson {
    using System;
    using System.Text;
    internal class JsonNullContract : IJsonContract {
        public object Create() => null;
        public object Parse(string value) {
            throw new NotImplementedException();
        }
        public void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            builder.Append("null");
        }
    }
}
