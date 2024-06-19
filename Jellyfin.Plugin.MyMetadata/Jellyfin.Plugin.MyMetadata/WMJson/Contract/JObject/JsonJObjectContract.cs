namespace WMJson {
    using System;
    using System.Text;
    internal class JsonJObjectContract : IJsonContract {
        public object Create() {
            throw new NotImplementedException();
        }
        public object Parse(string value) {
            throw new NotImplementedException();
        }
        public virtual void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            throw new NotImplementedException();
        }
    }
}
