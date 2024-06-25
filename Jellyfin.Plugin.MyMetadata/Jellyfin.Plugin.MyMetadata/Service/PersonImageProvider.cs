using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class PersonImageProvider<T> : BaseImageProvider<PersonImageProvider<T>, T, Person> where T : HttpService {
        public PersonImageProvider(ILogger<PersonImageProvider<T>> logger, T httpService) : base(logger, httpService) { }
        protected override async Task<IEnumerable<RemoteImageInfo>> GetImages_impl(BaseItem item, CancellationToken cancellationToken) {
            var id = item.GetProviderId(ProviderID);
            //获取影片详情
            var personInfo = await httpService.GetPersonAsync<PersonItem>(item.Name, cancellationToken);
            if (personInfo == null)
                throw new Exception("personInfo is null");
            var list = new List<RemoteImageInfo>();
            // 小封面 poster
            list.Add(new RemoteImageInfo {
                ProviderName = Name,
                Url = personInfo.ImageUrl,
                Type = ImageType.Primary,
                Language = Language,
            });
            // 大封面 fanart/backdrop
            list.Add(new RemoteImageInfo {
                ProviderName = Name,
                Url = personInfo.ImageUrl,
                Type = ImageType.Backdrop,
                Language = Language,
            });
            // 列表为“缩略图”显示时，显示大封面
            list.Add(new RemoteImageInfo {
                ProviderName = Name,
                Url = personInfo.ImageUrl,
                Type = ImageType.Thumb,
                Language = Language,
            });
            return list;
        }
        public override IEnumerable<ImageType> GetSupportedImages(BaseItem item) {
            yield return ImageType.Primary;
            yield return ImageType.Backdrop;
            yield return ImageType.Thumb;
        }
    }
}
