using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class MovieProvider : IRemoteMetadataProvider<Movie, MovieInfo> {
        private readonly ILogger<MovieProvider> logger;
        private readonly TestHttpService httpService;
        public string Name => Config.ProviderName;
        public MovieProvider(ILogger<MovieProvider> logger, TestHttpService httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }

        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        public async Task<MetadataResult<Movie>> GetMetadata(MovieInfo info, CancellationToken cancellationToken) {
            logger.LogInformation("===================GetMetadata " + info.Name + "  " + info.Path);
            var id = info.GetProviderId(Config.ProviderID);

            //// 如果 AvMoo Id 为空，则根据标题重新获取，且默认使用结果的第一条数据
            //if (string.IsNullOrWhiteSpace(id)) {
            //    //var results = await GetIdsAsync(info.Name, cancellationToken);
            //    var results = (await _http.SearchAsync<SearchResult>(info.Name, cancellationToken).ConfigureAwait(false)).Select(i => i.Id);

            //    if (results.Count() > 0) {
            //        id = results.FirstOrDefault();
            //    } else {
            //        return new MetadataResult<Movie>();
            //    }
            //}

            // 获取 元数据
            var movie = await httpService.GetMovieMetadataAsync(id, cancellationToken).ConfigureAwait(false);

            //if (movie != null && movie.HasMetadata) {
            //    // 如果能获取到元数据，则把 AvMoo Id 设置为 当前 id
            //    info.SetProviderId(Plugin.Instance.Configuration.Sod.ProviderId, id);
            //}

            return movie;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken) {
            
            var results = new List<RemoteSearchResult>();
            var Id = searchInfo.GetProviderId(ProviderID);
            logger.LogInformation("===================GetSearchResults " + searchInfo.Name + "  " + searchInfo.Path + "  " + "  " + Id + "  " + searchInfo.IndexNumber + "  " + searchInfo.ParentIndexNumber);
            var result = new RemoteSearchResult() {
                IndexNumber = searchInfo.IndexNumber,
                Name = "Title",
                ImageUrl = "ImageUrl",
                Overview = "Overview",
                ParentIndexNumber = searchInfo.ParentIndexNumber,
                SearchProviderName = "SearchProviderName"
            };
            results.Add(result);
            return results;
            //var ids = new List<string>();

            //if (!string.IsNullOrEmpty(AvmooId)) {
            //    // id 不为空，添加到 id 列表
            //    ids = new List<string>
            //    {
            //        AvmooId
            //    };
            //} else {
            //    // id 为空，则通过名称在线搜索并返回搜索结果的 id 列表
            //    //ids = (List<string>)await GetIdsAsync(searchInfo.Name, cancellationToken).ConfigureAwait(false);
            //    ids = (await _http.SearchAsync<SearchResult>(searchInfo.Name, cancellationToken).ConfigureAwait(false)).Select(i => i.Id).ToList();
            //    _logger.LogInformation($"search results count: {ids.Count}");
            //}

            //// 遍历 id 列表
            //foreach (string id in ids) {
            //    // 获取影片详情
            //    var item = await _http.GetMovieAsync<MovieItem>(id, cancellationToken).ConfigureAwait(false);

            //    // 转换为 Jellyfin 查找结果(RemoteSearchResult)对象
            //    var result = new RemoteSearchResult() {
            //        IndexNumber = searchInfo.IndexNumber,
            //        Name = item.Title,
            //        ImageUrl = item.Poster,
            //        Overview = item.Intro,
            //        ParentIndexNumber = searchInfo.ParentIndexNumber,
            //        SearchProviderName = Name
            //    };

            //    // 如果发行日期不为空，则设置年份
            //    if (item.ReleaseDate != null) {
            //        result.PremiereDate = item.ReleaseDate;
            //        result.ProductionYear = item.ReleaseDate?.Year;
            //    }

            //    _logger.LogInformation($"ProviderId: {Plugin.Instance.Configuration.Sod.ProviderId}");

            //    // 设置 id
            //    result.SetProviderId(Plugin.Instance.Configuration.Sod.ProviderId, id);

            //    // 添加到搜索结果列表
            //    results.Add(result);
            //}

            //return results;
        }
    }
}
