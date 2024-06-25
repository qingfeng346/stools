using MediaBrowser.Controller.Entities;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class StudioImageProvider<T> : BaseImageProvider<StudioImageProvider<T>, T, Studio> where T : HttpService {
        public StudioImageProvider(ILogger<StudioImageProvider<T>> logger, T httpService) : base(logger, httpService) { }
    }
}