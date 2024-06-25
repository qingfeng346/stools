using MediaBrowser.Controller.Entities.TV;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class EpisodeProvider<T> : BaseProvider<EpisodeProvider<T>, T, Episode, EpisodeInfo> where T : HttpService {
        public EpisodeProvider(ILogger<EpisodeProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}