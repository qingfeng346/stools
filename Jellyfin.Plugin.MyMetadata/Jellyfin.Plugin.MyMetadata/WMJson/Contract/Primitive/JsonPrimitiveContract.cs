namespace WMJson {
    using System;
    using System.Text;
    internal abstract class JsonPrimitiveContract : IJsonContract {
        public abstract object Parse(string value);
        public object Create() {
            throw new NotImplementedException();
        }
        public void SetValue(string key, object obj, object value) {
            throw new NotImplementedException();
        }
        public void AddValue(object obj, object value) {
            throw new NotImplementedException();
        }
        public virtual void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting) {
            builder.Append(value);
        }
    }
}
