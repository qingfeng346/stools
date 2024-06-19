namespace WMJson {
    internal class JsonInt8Contract : JsonPrimitiveContract {
        public override object Parse(string value) => sbyte.Parse(value);
    }
}
