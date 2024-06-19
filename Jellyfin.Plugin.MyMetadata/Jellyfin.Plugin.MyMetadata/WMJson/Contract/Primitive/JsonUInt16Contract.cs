namespace WMJson {
    internal class JsonUInt16Contract : JsonPrimitiveContract {
        public override object Parse(string value) => ushort.Parse(value);
    }
}
