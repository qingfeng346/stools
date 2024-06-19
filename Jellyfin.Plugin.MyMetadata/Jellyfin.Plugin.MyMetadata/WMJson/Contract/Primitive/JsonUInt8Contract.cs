namespace WMJson {
    internal class JsonUInt8Contract : JsonPrimitiveContract {
        public override object Parse(string value) => byte.Parse(value);
    }
}
