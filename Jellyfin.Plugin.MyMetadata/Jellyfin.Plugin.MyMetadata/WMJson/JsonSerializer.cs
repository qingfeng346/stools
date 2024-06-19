using System.Text;
namespace WMJson {
    public class JsonSerializer {
        internal StringBuilder m_Builder = new StringBuilder();
        public string Parse(object value, JsonSerializeSetting setting) {
            m_Builder.Length = 0;
            Serialize(value, setting ?? JsonSerializeSetting.Default);
            return m_Builder.ToString();
        }
        void Serialize(object value, JsonSerializeSetting setting) {
            JsonUtils.GetContract(value?.GetType()).ToJson(value, m_Builder, setting);
        }
    }
}
