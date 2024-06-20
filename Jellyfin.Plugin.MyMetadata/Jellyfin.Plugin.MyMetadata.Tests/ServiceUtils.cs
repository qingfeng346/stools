using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;

namespace Jellyfin.Plugin.MyMetadata.Tests
{
    class ServiceUtils
    {
        public static ServiceProvider BuildServiceProvider<T>(ITestOutputHelper output) where T : class
        {
            var services = new ServiceCollection()
                .AddHttpClient()
                .AddLogging(builder => builder.AddXUnit(output).SetMinimumLevel(LogLevel.Debug))
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddLogging(builder => builder.AddSystemdConsole().SetMinimumLevel(LogLevel.Debug))
                .AddSingleton<T>()
                .AddSingleton<Jellyfin.Plugin.MyMetadata.Service.Test.TestHttpService>()
                .AddSingleton<Jellyfin.Plugin.MyMetadata.Service.Test2.TestHttpService>()
                .AddSingleton<Jellyfin.Plugin.MyMetadata.Service.Test3.TestHttpService>();

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetService<ILogger<T>>();
            services.AddSingleton<ILogger>(logger);
            return services.BuildServiceProvider();
        }
    }
}
