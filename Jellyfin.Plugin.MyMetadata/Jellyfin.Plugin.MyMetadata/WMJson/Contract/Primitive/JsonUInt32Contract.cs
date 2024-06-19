namespace WMJson {
    internal class JsonUInt32Contract : JsonPrimitiveContract {
        public override object Parse(string value) => uint.Parse(value);
    }
}
