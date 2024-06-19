namespace WMJson {
    internal class JsonInt64Contract : JsonPrimitiveContract {
        public override object Parse(string value) => long.Parse(value);
    }
}
