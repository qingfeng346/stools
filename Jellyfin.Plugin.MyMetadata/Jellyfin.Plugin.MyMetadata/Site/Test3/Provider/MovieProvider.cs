using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service.Test3 {
    public class MovieProvider : MovieProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public MovieProvider(ILogger<MovieProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
}
