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
        public virtual async Task<HttpResponseMessage> GetResponseAsync(string url, CancellationToken cancellationToken) => 
                                                        await http.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        /// <summary>下载 HTMl 源码 </summary>
        public virtual async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken) => 
                                                        await http.CreateClient(NamedClient.Default).GetStringAsync(url, cancellationToken);
        /// <summary> 获取影片元数据 </summary>
        public abstract Task<MetadataResult<Movie>> GetMovieMetadataAsync(string id, CancellationToken cancellationToken);
        /// <summary> 查找影片 </summary>
        // public abstract Task<IEnumerable<T>> SearchAsync<T>(string keyword, CancellationToken cancellationToken) where T : SearchResult;
    }
}
