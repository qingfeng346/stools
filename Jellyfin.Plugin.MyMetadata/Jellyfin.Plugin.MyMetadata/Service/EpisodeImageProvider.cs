using MediaBrowser.Controller.Entities.TV;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class EpisodeImageProvider<T> : BaseImageProvider<EpisodeImageProvider<T>, T, Episode> where T : HttpService {
        public EpisodeImageProvider(ILogger<EpisodeImageProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}