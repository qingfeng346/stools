﻿using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using WMJson;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class PersonImageProvider<T> : IRemoteImageProvider where T : HttpService {
        protected readonly ILogger<PersonImageProvider<T>> logger;
        protected readonly T httpService;
        public abstract string Name { get; }
        public abstract string ProviderID { get; }
        public PersonImageProvider(ILogger<PersonImageProvider<T>> logger, T httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public async Task<IEnumerable<RemoteImageInfo>> GetImages(BaseItem item, CancellationToken cancellationToken) {
            var list = new List<RemoteImageInfo>();
            try {
                var id = item.GetProviderId(ProviderID);
                logger.LogInformation($"PersonImageProvider.GetImages Id:{id} ItemId:{item.Id} Name:{item.Name} Path:{item.Path}");
                //获取影片详情
                var personItem = await httpService.GetPersonAsync<PersonItem>(item.Name, cancellationToken);
                if (personItem == null)
                    return list;
                //如果存在大封面
                if (!string.IsNullOrEmpty(personItem.ImageUrl)) {
                    // 小封面 poster
                    list.Add(new RemoteImageInfo {
                        ProviderName = Name,
                        Url = personItem.ImageUrl,
                        Type = ImageType.Primary
                    });

                    // 大封面 fanart/backdrop
                    list.Add(new RemoteImageInfo {
                        ProviderName = Name,
                        Url = personItem.ImageUrl,
                        Type = ImageType.Backdrop
                    });

                    // 列表为“缩略图”显示时，显示大封面
                    list.Add(new RemoteImageInfo {
                        ProviderName = Name,
                        Url = personItem.ImageUrl,
                        Type = ImageType.Thumb
                    });
                }
                return list;
            } catch (System.Exception e) {
                logger.LogError($"PersonImageProvider.GetImages is error : {e}");
                return list;
            }
        }
        public IEnumerable<ImageType> GetSupportedImages(BaseItem item) {
            yield return ImageType.Primary;
            yield return ImageType.Art;
            yield return ImageType.Backdrop;
            yield return ImageType.Banner;
            yield return ImageType.Logo;
            yield return ImageType.Thumb;
            yield return ImageType.Disc;
            yield return ImageType.Box;
            yield return ImageType.Screenshot;
            yield return ImageType.Menu;
            yield return ImageType.Chapter;
            yield return ImageType.BoxRear;
            yield return ImageType.Profile;
        }
        public bool Supports(BaseItem item) {
            return item is Person;
        }
    }
}