using Jellyfin.Data.Enums;
using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
using System.Globalization;
namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class TestHttpService : HttpService {
        public TestHttpService(ILogger<HttpService> logger, IHttpClientFactory http) : base(logger, http) { }
        public override async Task<T> GetMovieAsync<T>(string url, CancellationToken cancellationToken) {
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var item = new MovieItem();
            item.Id = url.Substring(url.LastIndexOf(":") + 1);
            item.Title = rootNode.SelectSingleNode("//h1[@class='text-lg']")?.InnerText;
            item.Fanart = rootNode.SelectSingleNode("//img[@class='max-w-full max-h-full']").GetAttributeValue<string>("src", "");
            item.Poster = item.Fanart;
            var infoNodes = rootNode.SelectNodes("//div[@class='text-sm']/a[@class='link']");
            if (DateTime.TryParseExact(infoNodes[0].InnerText, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                item.ReleaseDate = releaseDate;
            item.Studios.Add(infoNodes[1].InnerText);
            item.Labels.Add(infoNodes[2].InnerText);
            item.Series.Add(infoNodes[3].InnerText);
            var personNodes = rootNode.SelectNodes("//a[@class='chip']");
            foreach (var node in personNodes) {
                var name = node.InnerText;
                var avatarUrl = node.SelectSingleNode("//div[@class='avatar']/div/img").GetAttributeValue<string>("src", "");
                item.Persons.Add(new PersonInfo() { 
                    Name = name,
                    Role = name,
                    Type = PersonKind.Actor,
                    ImageUrl = avatarUrl
                });
            }
            var genreNodes = rootNode.SelectNodes("//a[@class='rounded-lg border border-solid text-sm px-2 py-1']");
            foreach (var node in genreNodes) {
                item.Genres.Add(node.InnerText);
            }
            var shotscreensNodes = rootNode.SelectNodes("//img[@class='h-28']");
            foreach (var node in shotscreensNodes) {
                item.Shotscreens.Add(node.GetAttributeValue<string>("src", ""));
            }
            item.SourceUrl = url;
            return item as T;
        }
        public override Task<(MetadataResult<Movie>, string)> GetMovieMetadataByNameAsync(string name, string id, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        public override async Task<IList<SearchResult>> SearchAsync(string keyword, CancellationToken cancellationToken) {
            //// 查询
            var html = await GetHtmlAsync($"https://www.avbase.net/works?q={keyword}", cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var nodes = rootNode.SelectNodes("//div[@class='grow']/a");
            logger.LogInformation($"搜索结果数量 : {nodes?.Count}");
            var results = new List<SearchResult>();
            foreach (var node in nodes) {
                var url = node.GetAttributeValue<string>("href", "");
                results.Add(new SearchResult() { Id = $"https://www.avbase.net{url}" });
            }
            return results;
        }
    }
}
