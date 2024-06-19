namespace WMJson {
    internal class JsonInt32Contract : JsonPrimitiveContract {
        public override object Parse(string value) => int.Parse(value);
    }
}
