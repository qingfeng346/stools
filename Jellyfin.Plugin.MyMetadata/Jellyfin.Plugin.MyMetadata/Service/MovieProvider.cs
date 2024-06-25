using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class MovieProvider<T> : BaseProvider<MovieProvider<T>, T, Movie, MovieInfo> where T : HttpService {
        public MovieProvider(ILogger<MovieProvider<T>> logger, T httpService) : base(logger, httpService) { }
        protected override async Task<MetadataResult<Movie>> GetMetadata_impl(MovieInfo info, CancellationToken cancellationToken) {
            var id = info.GetProviderId(ProviderID);
            var name = Path.GetFileNameWithoutExtension(info.Path);
            var movieId = await httpService.GetMovieIdByName(name, id, "", cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrEmpty(movieId))
                throw new Exception("movieId is null");
            var movieInfo = await httpService.GetMovieMetadataAsync(movieId, cancellationToken).ConfigureAwait(false);
            if (movieInfo == null)
                throw new Exception("movieInfo is null");
            if (movieInfo.HasMetadata)
                info.SetProviderId(ProviderID, movieId);
            return movieInfo;
        }
        protected override async Task<IEnumerable<RemoteSearchResult>> GetSearchResults_impl(MovieInfo searchInfo, CancellationToken cancellationToken) {
            var id = searchInfo.GetProviderId(ProviderID);
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
        }
    }
}
