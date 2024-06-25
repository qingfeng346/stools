using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.Providers;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public static class Config {
        public const string ProviderName = "MyMetadata";
        public const string ProviderID = "MyMetadataID";
    }
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
    public class ServiceRegistrator : IPluginServiceRegistrator {
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost) {
            // 注入
            serviceCollection.AddSingleton<TestHttpService>();
        }
    }
    public class MovieProvider : MovieProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public MovieProvider(ILogger<MovieProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class MovieImageProvider : MovieImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public MovieImageProvider(ILogger<MovieImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class PersonProvider : PersonProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public PersonProvider(ILogger<PersonProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class PersonImageProvider : PersonImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public PersonImageProvider(ILogger<PersonImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class SeasonProvider : SeasonProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public SeasonProvider(ILogger<SeasonProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class SeasonImageProvider : SeasonImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public SeasonImageProvider(ILogger<SeasonImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class SeriesProvider : SeriesProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public SeriesProvider(ILogger<SeriesProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class SeriesImageProvider : SeriesImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public SeriesImageProvider(ILogger<SeriesImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class EpisodeProvider : EpisodeProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public EpisodeProvider(ILogger<EpisodeProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
    public class EpisodeImageProvider : EpisodeImageProvider<TestHttpService> {
        public override string Name => Config.ProviderName;
        public override string ProviderID => Config.ProviderID;
        public EpisodeImageProvider(ILogger<EpisodeImageProvider> logger, TestHttpService httpService) : base(logger, httpService) { }
    }
}
