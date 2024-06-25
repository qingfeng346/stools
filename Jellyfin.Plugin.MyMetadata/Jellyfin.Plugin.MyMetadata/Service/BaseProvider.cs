using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class BaseProvider<T1, T2, TItemType, TLookupInfoType> : IRemoteMetadataProvider<TItemType, TLookupInfoType>
            where T2 : HttpService
            where TItemType : BaseItem, IHasLookupInfo<TLookupInfoType>
            where TLookupInfoType : ItemLookupInfo, new() {
        protected readonly ILogger<T1> logger;
        protected readonly T2 httpService;
        public abstract string Name { get; }
        public abstract string ProviderID { get; }
        public BaseProvider(ILogger<T1> logger, T2 httpService) {
            this.logger = logger;
            this.httpService = httpService;
        }
        public async Task<HttpResponseMessage> GetImageResponse(string url, CancellationToken cancellationToken) => await httpService.GetResponseAsync(url, cancellationToken);
        string GetString(TLookupInfoType info) {
            var id = info.GetProviderId(ProviderID);
            return $"Id:{id} Name:{info.Name} Path:{info.Path} OriginalTitle:{info.OriginalTitle}";
        }
        public async Task<MetadataResult<TItemType>> GetMetadata(TLookupInfoType info, CancellationToken cancellationToken) {
            try {
                logger.LogInformation($"[{GetType().Name}] Start GetMetadata {GetString(info)}");
                return await GetMetadata_impl(info, cancellationToken);
            } catch (Exception e) {
                logger.LogError($"[{GetType().Name}] GetMetadata is error : {e.Message}");
                return new MetadataResult<TItemType>();
            }
        }
        public async Task<IEnumerable<RemoteSearchResult>> GetSearchResults(TLookupInfoType info, CancellationToken cancellationToken) {
            try {
                logger.LogInformation($"[{GetType().Name}] Start GetSearchResults {GetString(info)}");
                return await GetSearchResults_impl(info, cancellationToken);
            } catch (Exception e) {
                logger.LogError($"[{GetType().Name}] GetSearchResults is error : {e.Message}");
                return Array.Empty<RemoteSearchResult>();
            }
        }
        protected virtual async Task<MetadataResult<TItemType>> GetMetadata_impl(TLookupInfoType info, CancellationToken cancellationToken) {
            return new MetadataResult<TItemType>();
        }
        protected virtual async Task<IEnumerable<RemoteSearchResult>> GetSearchResults_impl(TLookupInfoType searchInfo, CancellationToken cancellationToken) {
            return Array.Empty<RemoteSearchResult>();
        }
    }
}
