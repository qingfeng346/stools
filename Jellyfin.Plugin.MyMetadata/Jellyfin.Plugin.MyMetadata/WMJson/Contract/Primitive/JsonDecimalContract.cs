namespace WMJson {
    internal class JsonDecimalContract : JsonPrimitiveContract {
        public override object Parse(string value) => decimal.Parse(value);
    }
}
