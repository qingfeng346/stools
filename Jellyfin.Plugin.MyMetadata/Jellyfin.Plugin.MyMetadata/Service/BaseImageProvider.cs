using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class BaseImageProvider<T1, T2, TItemType> : IRemoteImageProvider
            where T2 : HttpService
            where TItemType : BaseItem {
        protected readonly ILogger<T1> logger;
        protected readonly T2 httpService;
        protected static IEnumerable<ImageType> EmptyImageType = Array.Empty<ImageType>();
        protected static IEnumerable<ImageType> AllImageType { get; } = new ImageType[] {
            ImageType.Primary,
            ImageType.Art,
            ImageType.Backdrop,
            ImageType.Banner,
            ImageType.Logo,
            ImageType.Thumb,
            ImageType.Disc,
            ImageType.Box,
            ImageType.Screenshot,
            ImageType.Menu,
            ImageType.Chapter,
            ImageType.BoxRear,
            ImageType.Profile,
        };
        public abstract string Name { get; }
        public abstract string ProviderID { get; }
        public string Language {get;} = "Japanese";
        public BaseImageProvider(ILogger<T1> logger, T2 httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public virtual IEnumerable<ImageType> GetSupportedImages(BaseItem item) => EmptyImageType;
        public bool Supports(BaseItem item) => item is TItemType;
        string GetString(BaseItem info) {
            var id = info.GetProviderId(ProviderID);
            return $"Id:{id} Name:{info.Name} Path:{info.Path} OriginalTitle:{info.OriginalTitle}";
        }
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken) {
            try {
                logger.LogInformation($"[{GetType().Name}] Start GetImages {GetString(item)}");
                return await GetImages_impl(item, cancellationToken);
            } catch (Exception e) {
                logger.LogError($"[{GetType().Name}] GetImages is error : {e.Message}");
                return null;
            }
        }
        protected virtual async Task<IEnumerable<RemoteImageInfo>> GetImages_impl(BaseItem item, CancellationToken cancellationToken) {
            return null;
        }
    }
}
