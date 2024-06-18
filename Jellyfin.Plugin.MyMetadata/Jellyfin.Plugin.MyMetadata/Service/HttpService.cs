using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class HttpService {
        protected readonly IHttpClientFactory http;
        public HttpService(ILogger<HttpService> logger, IHttpClientFactory http) {
            this.http = http;
        }
        /// <summary>
        /// 下载 HTMl 源码
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken) {
            return await http.CreateClient(NamedClient.Default).GetStringAsync(url, cancellationToken);
        }

        /// <summary>
        /// 从 HTML 中获取影片信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //public abstract Task<T> GetMovieAsync<T>(string id, CancellationToken cancellationToken) where T : MovieItem;

        /// <summary>
        /// 获取影片元数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<MetadataResult<Movie>> GetMovieMetadataAsync(string id, CancellationToken cancellationToken);

        /// <summary>
        /// 获取响应对象
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual async Task<HttpResponseMessage> GetResponseAsync(string url, CancellationToken cancellationToken) => await http.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);

        /// <summary>
        /// 查找影片
        /// </summary>
        /// <param name="keyword">要查找的关键字(一般为识别码)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //public abstract Task<IEnumerable<T>> SearchAsync<T>(string keyword, CancellationToken cancellationToken) where T : SearchResult;
    }
}
