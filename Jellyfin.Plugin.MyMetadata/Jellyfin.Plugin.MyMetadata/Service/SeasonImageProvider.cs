using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class SeasonImageProvider<T> : BaseImageProvider<SeasonImageProvider<T>, T, Season> where T : HttpService {
        public SeasonImageProvider(ILogger<SeasonImageProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}
