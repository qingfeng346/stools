using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    /// <summary>
    /// Register services.
    /// </summary>
    public class ServiceRegistrator : IPluginServiceRegistrator {
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost) {
            // 注入
            serviceCollection.AddSingleton<TestHttpService>();
        }
    }
}
