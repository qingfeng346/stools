using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class SeasonProvider<T> : BaseProvider<SeasonProvider<T>, T, Season, SeasonInfo> where T : HttpService {
        public SeasonProvider(ILogger<SeasonProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}
