namespace WMJson {
    internal class JsonUInt64Contract : JsonPrimitiveContract {
        public override object Parse(string value) => ulong.Parse(value);
    }
}
