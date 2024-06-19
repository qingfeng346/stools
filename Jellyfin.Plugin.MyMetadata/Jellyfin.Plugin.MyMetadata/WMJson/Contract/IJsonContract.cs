using System.Text;
namespace WMJson {
    internal interface IJsonContract {
        object Parse(string value);
        object Create();
        void ToJson(object value, StringBuilder builder, JsonSerializeSetting setting);
    }
}
