using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class HttpService {
        protected readonly ILogger<HttpService> logger;
        protected readonly IHttpClientFactory http;
        public HttpService(ILogger<HttpService> logger, IHttpClientFactory http) {
            this.logger = logger;
            this.http = http;
        }
        /// <summary>获取响应对象 </summary>
        public async Task<HttpResponseMessage> GetResponseAsync(string url, CancellationToken cancellationToken) {
            logger.LogInformation($"GetResponseAsync : {url}");
            return await http.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        } 
        /// <summary>下载 HTMl 源码 </summary>
        public async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken) {
            logger.LogInformation($"GetHtmlAsync : {url}");
            return await http.CreateClient(NamedClient.Default).GetStringAsync(url, cancellationToken);
        }
        /// <summary> 获取影片元数据 </summary>
        public async Task<MetadataResult<Movie>> GetMovieMetadataAsync(string id, CancellationToken cancellationToken) {
            await Task.Delay(1);
            var result = new MetadataResult<Movie>();
            //// 获取影片详情
            var item = await GetMovieAsync<MovieItem>(id, cancellationToken);
            // 设置 基础信息
            var movie = new Movie {
                Name = $"{item.Id} {item.Title}",
                OriginalTitle = item.Title,
                HomePageUrl = id,
            };
            // 如果 系列 不为空
            if (item.Series.Count > 0)
                movie.CollectionName = item.Series[0];
            // 如果 发行日期 不为空
            if (item.ReleaseDate != null) {
                var releaseDate = item.ReleaseDate?.ToUniversalTime();
                // 设置 发行日期
                movie.PremiereDate = releaseDate;
                // 设置 年份
                movie.ProductionYear = releaseDate?.Year;
            }
            // 添加类别
            item.Genres?.ForEach(movie.AddGenre);
            // 添加 工作室
            item.Studios?.ForEach(movie.AddStudio);
            // 添加 发行商
            item.Labels?.ForEach(movie.AddStudio);
            // 演员
            item.Persons.ForEach(result.AddPerson);
            result.QueriedById = false;
            result.HasMetadata = true;
            result.Item = movie;
            return result;
        }
        public abstract Task<(MetadataResult<Movie>, string)> GetMovieMetadataByNameAsync(string name, string id, CancellationToken cancellationToken);
        public abstract Task<T> GetMovieAsync<T>(string id, CancellationToken cancellationToken) where T : MovieItem;
        public abstract Task<IList<SearchResult>> SearchAsync(string keyword, CancellationToken cancellationToken);
    }
}
