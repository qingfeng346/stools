using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class MovieImageProvider<T> : BaseImageProvider<MovieImageProvider<T>, T, Movie> where T : HttpService {
        public MovieImageProvider(ILogger<MovieImageProvider<T>> logger, T httpService) : base(logger, httpService) { }
        protected override async Task<IEnumerable<RemoteImageInfo>> GetImages_impl(BaseItem item, CancellationToken cancellationToken) {
            var list = new List<RemoteImageInfo>();
            var id = item.GetProviderId(ProviderID);
            var movieId = await httpService.GetMovieIdByName(Path.GetFileNameWithoutExtension(item.Path), id, "", cancellationToken);
            if (string.IsNullOrWhiteSpace(movieId))
                return list;
            //获取影片详情
            var movieInfo = await httpService.GetMovieAsync<MovieItem>(movieId, cancellationToken);
            if (movieInfo == null)
                return list;
            //如果存在大封面
            if (!string.IsNullOrEmpty(movieInfo.ImageUrl)) {
                // 小封面 poster
                list.Add(new RemoteImageInfo {
                    ProviderName = Name,
                    Url = movieInfo.ThumbUrl,
                    ThumbnailUrl = movieInfo.ThumbUrl,
                    Type = ImageType.Primary,
                    Language = Language,
                });
                // 大封面 fanart/backdrop
                list.Add(new RemoteImageInfo {
                    ProviderName = Name,
                    Url = movieInfo.ImageUrl,
                    ThumbnailUrl = movieInfo.ImageUrl,
                    Type = ImageType.Backdrop,
                    Language = Language,
                });
                // 大封面 fanart/backdrop
                list.Add(new RemoteImageInfo {
                    ProviderName = Name,
                    Url = movieInfo.ImageUrl,
                    ThumbnailUrl = movieInfo.ImageUrl,
                    Type = ImageType.Banner,
                    Language = Language,
                });
                // 列表为“缩略图”显示时，显示大封面
                list.Add(new RemoteImageInfo {
                    ProviderName = Name,
                    Url = movieInfo.ImageUrl,
                    ThumbnailUrl = movieInfo.ImageUrl,
                    Type = ImageType.Thumb,
                    Language = Language,
                });
            }
            return list;
        }
        public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) {
            yield return ImageType.Primary;
            yield return ImageType.Backdrop;
            yield return ImageType.Banner;
            yield return ImageType.Thumb;
        }
    }
}
