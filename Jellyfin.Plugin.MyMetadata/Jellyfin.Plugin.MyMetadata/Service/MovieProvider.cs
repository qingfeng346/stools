using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
using WMJson;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class MovieProvider<T> : IRemoteMetadataProvider<Movie, MovieInfo> where T : HttpService {
        private readonly ILogger<MovieProvider<T>> logger;
        private readonly T httpService;
        public abstract string Name { get; }
        public abstract string ProviderID { get; }
        public MovieProvider(ILogger<MovieProvider<T>> logger, T httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken) {
            var id = info.GetProviderId(ProviderID);
            logger.LogInformation($"GetMetadata Id:{id} Info:{JsonConvert.Serialize(info.Name)}");
            var name = Path.GetFileNameWithoutExtension(info.Path);
            var (movie, Id) = await httpService.GetMovieMetadataByNameAsync(name, id, cancellationToken).ConfigureAwait(false);
            if (movie != null && movie.HasMetadata && !string.IsNullOrEmpty(Id)) {
               info.SetProviderId(ProviderID, Id);
            }
            return movie;
        }
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken) {
            var id = searchInfo.GetProviderId(ProviderID);
            logger.LogInformation($"GetSearchResults Id:{id} Info:{JsonConvert.Serialize(searchInfo)}");
            var results = new List<RemoteSearchResult>();
            var searchResults = await httpService.SearchAsync(searchInfo.Name, cancellationToken).ConfigureAwait(false);
            if (searchResults == null || searchResults.Count == 0) return results;
            // 遍历 id 列表
            foreach (var searchResult in searchResults) {
                // 获取影片详情
                var item = await httpService.GetMovieAsync<MovieItem>(searchResult.Id, cancellationToken).ConfigureAwait(false);
                // 转换为 Jellyfin 查找结果(RemoteSearchResult)对象
                var result = new RemoteSearchResult() {
                    Name = item.Title,
                    ImageUrl = item.Poster,
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
        }
    }
}
