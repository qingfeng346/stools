using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class PersonImageProvider : PersonImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public PersonImageProvider(ILogger<PersonImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
}
