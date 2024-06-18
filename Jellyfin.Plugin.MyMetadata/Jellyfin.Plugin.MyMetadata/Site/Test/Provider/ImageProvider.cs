using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class ImageProvider : IRemoteImageProvider {
        private readonly ILogger<ImageProvider> logger;
        private readonly TestHttpService httpService;
        public string Name => Config.ProviderName;
        public ImageProvider(ILogger<ImageProvider> logger, TestHttpService httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken) {
            var list = new List<RemoteImageInfo>();
            var id = item.GetProviderId(Config.ProviderID);
            if (string.IsNullOrWhiteSpace(id)) {
                //_logger.LogWarning($"GetImages failed because that the sid is empty: {item.Name}");
                return list;
            }

            // 获取影片详情
            //var movie = await httpService.GetMovieAsync<MovieItem>(id, cancellationToken);

            // 如果存在大封面
            //if (!string.IsNullOrEmpty(movie.Fanart)) {
            //    // 小封面 poster
            //    list.Add(new RemoteImageInfo {
            //        ProviderName = Name,
            //        Url = movie.Poster,
            //        Type = ImageType.Primary
            //    });

            //    // 大封面 fanart/backdrop
            //    list.Add(new RemoteImageInfo {
            //        ProviderName = Name,
            //        Url = movie.Fanart,
            //        Type = ImageType.Backdrop
            //    });

            //    // 列表为“缩略图”显示时，显示大封面
            //    list.Add(new RemoteImageInfo {
            //        ProviderName = Name,
            //        Url = movie.Fanart,
            //        Type = ImageType.Thumb
            //    });
            //}

            //// 添加预览图
            //movie.Shotscreens?.ForEach(img => {
            //    list.Add(new RemoteImageInfo {
            //        ProviderName = Plugin.Instance.Configuration.Avmoo.ProviderName,
            //        Url = img,
            //        Type = ImageType.Screenshot,
            //        ThumbnailUrl = img // 缩略文件名
            //    });
            //});
            return list;
        }
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item) {
            yield return ImageType.Primary;
            yield return ImageType.Backdrop;
            yield return ImageType.Screenshot;
            yield return ImageType.Thumb;
        }
        public bool Supports(BaseItem item) {
            return item is Movie;
        }
    }
}
