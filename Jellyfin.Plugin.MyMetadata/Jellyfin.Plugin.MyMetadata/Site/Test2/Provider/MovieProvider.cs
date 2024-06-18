using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.MyMetadata.Service.Test2 {
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
            logger.LogInformation($"GetMetadata : {info.Name}  Path : {info.Path}");
            var name = Path.GetFileNameWithoutExtension(info.Path);
            var results = await httpService.SearchAsync(name, cancellationToken).ConfigureAwait(false);
            var url = results.FirstOrDefault()?.Id;
            if (string.IsNullOrEmpty(url)) {
                return new MetadataResult<Movie>();
            }
            // 获取 元数据
            var movie = await httpService.GetMovieMetadataAsync(url, cancellationToken).ConfigureAwait(false);
            if (movie != null && movie.HasMetadata) {
               info.SetProviderId(Config.ProviderID, url);
            }
            return movie;
        }

        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(MovieInfo searchInfo, CancellationToken cancellationToken) {
            logger.LogInformation($"GetSearchResults : {searchInfo.Name}");
            var results = new List<RemoteSearchResult>();
            var Id = searchInfo.GetProviderId(Config.ProviderID);
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
        }
    }
}
