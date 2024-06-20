using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using WMJson;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class PersonProvider<T> : IRemoteMetadataProvider<Person, PersonLookupInfo> where T : HttpService {
        protected readonly ILogger<PersonProvider<T>> logger;
        protected readonly T httpService;
        public abstract string Name { get; }
        public abstract string ProviderID { get; }
        public PersonProvider(ILogger<PersonProvider<T>> logger, T httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public async Task<MetadataResult<Person>> GetMetadata(PersonLookupInfo info, CancellationToken cancellationToken) {
            try {
                var id = info.GetProviderId(ProviderID);
                logger.LogInformation($"PersonProvider.GetMetadata Id:{id} Name:{info.Name} Path:{info.Path}");
                var person = await httpService.GetPersonMetadataAsync(id, info.Name, cancellationToken).ConfigureAwait(false);
                if (person == null)
                    return new MetadataResult<Person>();
                // if (person != null && person.HasMetadata)
                //     info.SetProviderId(ProviderID, movieId);
                return person;
            } catch (Exception e) {
                logger.LogError($"MovieProvider.GetMetadata is error : {e}");
                return new MetadataResult<Person>();
            }
        }
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(PersonLookupInfo searchInfo, CancellationToken cancellationToken) {
            var id = searchInfo.GetProviderId(ProviderID);
            var results = new List<RemoteSearchResult>();
            try {
                logger.LogInformation($"GetSearchResults Id:{id} Info:{JsonConvert.Serialize(searchInfo)}");
                var searchResults = await httpService.SearchAsync(searchInfo.Name, cancellationToken).ConfigureAwait(false);
                if (searchResults == null || searchResults.Count == 0) return results;
                // 遍历 id 列表
                foreach (var searchResult in searchResults) {
                    // 获取影片详情
                    var item = await httpService.GetMovieAsync<MovieItem>(searchResult.Id, cancellationToken).ConfigureAwait(false);
                    // 转换为 Jellyfin 查找结果(RemoteSearchResult)对象
                    var result = new RemoteSearchResult() {
                        Name = item.Title,
                        ImageUrl = item.ThumbUrl,
                        Overview = item.Intro,
                        IndexNumber = searchInfo.IndexNumber,
                        ParentIndexNumber = searchInfo.ParentIndexNumber,
                        SearchProviderName = Name
                    };
                    // 如果发行日期不为空，则设置年份
                    if (item.ReleaseDate != null) {
                        result.PremiereDate = item.ReleaseDate;
                        result.ProductionYear = item.ReleaseDate?.Year;
                    }
                    result.SetProviderId(ProviderID, searchResult.Id);
                    results.Add(result);
                }
                return results;
            } catch (Exception e) {
                logger.LogError($"MovieProvider.GetSearchResults is error : {e}");
                return results;
            }
        }
    
    }
}
