namespace WMJson {
    internal class JsonInt16Contract : JsonPrimitiveContract {
        public override object Parse(string value) => short.Parse(value);
    }
}
