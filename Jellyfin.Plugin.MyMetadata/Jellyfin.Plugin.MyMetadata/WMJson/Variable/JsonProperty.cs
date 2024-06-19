using System.Reflection;
namespace WMJson {
    internal class JsonProperty : JsonVariable {
        private PropertyInfo propertyInfo;
        public override bool CanRead => propertyInfo.CanRead;
        public override bool CanWrite => propertyInfo.CanWrite;
        public JsonProperty(PropertyInfo propertyInfo) : base(propertyInfo.Name, propertyInfo.PropertyType) {
            this.propertyInfo = propertyInfo;
        }
        public override void SetValue(object obj, object value) => propertyInfo.SetValue(obj, value, null);
        public override object GetValue(object obj) => propertyInfo.GetValue(obj);
    }
}
