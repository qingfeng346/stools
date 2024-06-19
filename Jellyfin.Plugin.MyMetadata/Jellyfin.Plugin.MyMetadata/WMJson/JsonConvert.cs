namespace WMJson {
    using System;
    public static class JsonConvert {
        public static T Deserialize<T>(string value, JsonDeserializeSetting setting = null) {
            return (T)Deserialize(value, typeof(T), setting);
        }
        public static object Deserialize(string value, JsonDeserializeSetting setting = null) {
            return Deserialize(value, typeof(object), setting);
        }
        public static object Deserialize(string value, Type type, JsonDeserializeSetting setting = null) {
            return new JsonDeserializer().Parse(value, type, setting);
        }
        public static string Serialize(object value, JsonSerializeSetting setting = null) {
            return new JsonSerializer().Parse(value, setting);
        }
        public static void Clean() {
            JsonUtils.ClearCache();
        }
    }

}
