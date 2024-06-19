using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using WMJson;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class ImageProvider<T> : IRemoteImageProvider where T : HttpService {
        protected readonly ILogger<ImageProvider<T>> logger;
        protected readonly T httpService;
        public abstract string Name { get; }
        public abstract string ProviderID { get; }
        public ImageProvider(ILogger<ImageProvider<T>> logger, T httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken) {
            var id = item.GetProviderId(ProviderID);
            logger.LogInformation($"GetImages Id:{id} Info:{JsonConvert.Serialize(item)}");
            var list = new List<RemoteImageInfo>();
            if (string.IsNullOrWhiteSpace(id)) {
                //_logger.LogWarning($"GetImages failed because that the sid is empty: {item.Name}");
                return list;
            }
            //获取影片详情
            var movie = await httpService.GetMovieAsync<MovieItem>(id, cancellationToken);
            //如果存在大封面
            if (!string.IsNullOrEmpty(movie.Fanart)) {
               // 小封面 poster
               list.Add(new RemoteImageInfo {
                   ProviderName = Name,
                   Url = movie.Poster,
                   Type = ImageType.Primary
               });

               // 大封面 fanart/backdrop
               list.Add(new RemoteImageInfo {
                   ProviderName = Name,
                   Url = movie.Fanart,
                   Type = ImageType.Backdrop
               });

               // 列表为“缩略图”显示时，显示大封面
               list.Add(new RemoteImageInfo {
                   ProviderName = Name,
                   Url = movie.Fanart,
                   Type = ImageType.Thumb
               });
            }
            // 添加预览图
            movie.Shotscreens?.ForEach(img => {
               list.Add(new RemoteImageInfo {
                   ProviderName = Name,
                   Url = img,
                   Type = ImageType.Screenshot,
                   ThumbnailUrl = img // 缩略文件名
               });
            });
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
