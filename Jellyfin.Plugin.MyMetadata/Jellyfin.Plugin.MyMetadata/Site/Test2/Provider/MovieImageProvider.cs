using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service.Test2 {
    public class MovieImageProvider : MovieImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public MovieImageProvider(ILogger<MovieImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
}
