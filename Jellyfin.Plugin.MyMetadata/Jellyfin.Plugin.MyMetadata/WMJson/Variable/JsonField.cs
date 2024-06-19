using System.Reflection;
namespace WMJson {
    internal class JsonField : JsonVariable {
        private FieldInfo fieldInfo;
        public override bool CanRead => true;
        public override bool CanWrite => true;
        public JsonField(FieldInfo fieldInfo) : base(fieldInfo.Name, fieldInfo.FieldType) {
            this.fieldInfo = fieldInfo;
        }
        public override void SetValue(object obj, object value) => fieldInfo.SetValue(obj, value);
        public override object GetValue(object obj) => fieldInfo.GetValue(obj);
    }
}
