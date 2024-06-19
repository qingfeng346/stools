using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
using WMJson;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class HttpService {
        protected readonly ILogger<HttpService> logger;
        protected readonly IHttpClientFactory http;
        protected readonly List<(string, MovieItem)> cacheMovies;
        public HttpService(ILogger<HttpService> logger, IHttpClientFactory http) {
            this.logger = logger;
            this.http = http;
            this.cacheMovies = new List<(string, MovieItem)>();
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
        /// <summary> 获取影片元数据 </summary>
        public async Task<T> GetMovieAsync<T>(string id, CancellationToken cancellationToken) where T : MovieItem {
            foreach (var (movieId, movieItem) in cacheMovies) {
                if (movieId == id)
                    return movieItem as T;
            }
            var item = await GetMovieAsync_impl<T>(id, cancellationToken);
            logger.LogInformation($"GetMovieAsync {id} Result : {JsonConvert.Serialize(item)}");
            cacheMovies.Add((id, item));
            if (cacheMovies.Count > 10)
                cacheMovies.RemoveAt(0);
            return item;
        }
        public async Task<IList<SearchResult>> SearchAsync(string keyword, CancellationToken cancellationToken) {
            var results = await SearchAsync_impl(keyword, cancellationToken);
            logger.LogInformation($"SearchAsync {keyword} Result : {JsonConvert.Serialize(results)}");
            return results;
        }
        public async Task<string> GetMovieIdByName(string name, string id, CancellationToken cancellationToken) {
            var result = await GetMovieIdByName_impl(name, id, cancellationToken);
            logger.LogInformation($"GetMovieIdByName name:{name} id:{id}  Result : {result}");
            return result;
        }
        protected abstract Task<T> GetMovieAsync_impl<T>(string id, CancellationToken cancellationToken) where T : MovieItem;
        protected abstract Task<IList<SearchResult>> SearchAsync_impl(string keyword, CancellationToken cancellationToken);
        protected virtual async Task<string> GetMovieIdByName_impl(string name, string id, CancellationToken cancellationToken) {
            return name;
        }
    }
}
