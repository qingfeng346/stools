using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class ExternalIdMovie : IExternalId {
        public string ProviderName => Config.ProviderName;
        public string Key => Config.ProviderID;
        public ExternalIdMediaType? Type => null;
        public string UrlFormatString => "https://www.avbase.net/works/{0}";
        public bool Supports(IHasProviderIds item) {
            return item is Movie || item is Video;
        }
    }
    public class ExternalIdPerson : IExternalId {
        public string ProviderName => Config.ProviderName;
        public string Key => Config.ProviderID;
        public ExternalIdMediaType? Type => null;
        public string UrlFormatString => "https://www.avbase.net/talents/{0}";
        public bool Supports(IHasProviderIds item) {
            return item is Person;
        }
    }
}
