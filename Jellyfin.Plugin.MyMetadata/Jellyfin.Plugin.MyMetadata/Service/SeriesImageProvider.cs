using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class SeriesImageProvider<T> : BaseImageProvider<SeriesImageProvider<T>, T, Series> where T : HttpService {
        public SeriesImageProvider(ILogger<SeriesImageProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}