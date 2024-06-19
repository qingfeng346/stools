namespace WMJson {
    internal class JsonFloatContract : JsonPrimitiveContract {
        public override object Parse(string value) => float.Parse(value);
    }
}
