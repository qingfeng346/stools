namespace WMJson {
    internal class JsonDoubleContract : JsonPrimitiveContract {
        public override object Parse(string value) => double.Parse(value);
    }
}
