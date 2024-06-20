using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class PersonProvider : PersonProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public PersonProvider(ILogger<PersonProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
}
