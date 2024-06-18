using MediaBrowser.Model.Plugins;
namespace Jellyfin.Plugin.MyMetadata.Configuration {
    public class PluginConfiguration : BasePluginConfiguration {
        public string Url { get; set; }
        public PluginConfiguration() {
        }
    }
}
