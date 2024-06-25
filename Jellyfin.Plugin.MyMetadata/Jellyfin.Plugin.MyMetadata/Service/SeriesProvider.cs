using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class SeriesProvider<T> : BaseProvider<SeriesProvider<T>, T, Series, SeriesInfo> where T : HttpService {
        public SeriesProvider(ILogger<SeriesProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}
