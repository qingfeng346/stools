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
        protected override async Task<T> GetMovieAsync_impl<T>(string id, CancellationToken cancellationToken) {
            var url = $"https://www.avbase.net/works/{id}";
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var item = new MovieItem();
            item.MovieId = url.Substring(url.LastIndexOf(":") + 1);
            item.Title = rootNode.SelectSingleNode("//h1[@class='text-lg']")?.InnerText;
            item.Fanart = rootNode.SelectSingleNode("//img[@class='max-w-full max-h-full']").GetAttributeValue<string>("src", "");
            item.Poster = item.Fanart;
            var infoNodes = rootNode.SelectNodes("//div[@class='bg-base-100 p-2 flex flex-col gap-1']");
            foreach (var node in infoNodes) {
                var nameNode = node.SelectSingleNode("div[@class='text-xs']");
                if (nameNode == null) continue;
                var valueNode = node.SelectSingleNode("div[@class='text-sm']");
                if (valueNode == null) continue;
                logger.LogInformation($"解析信息 {nameNode.InnerText} = {valueNode.InnerText}");
                switch (nameNode.InnerText) {
                    case "発売日":
                        if (DateTime.TryParseExact(valueNode.InnerText, "yyyy/MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                            item.ReleaseDate = releaseDate;
                        break;
                    case "メーカー":
                        item.Studios.Add(valueNode.InnerText);
                        break;
                    case "レーベル":
                        item.Labels.Add(valueNode.InnerText);
                        break;
                    case "シリーズ":
                        item.Series.Add(valueNode.InnerText);
                        break;
                }
            }
            var personNodes = rootNode.SelectNodes("//a[@class='chip']");
            foreach (var node in personNodes) {
                var name = node.InnerText;
                var avatarUrl = node.SelectSingleNode("//div[@class='avatar']/div/img").GetAttributeValue<string>("src", "");
                item.Persons.Add(new PersonInfo() { 
                    Name = name,
                    Role = name,
                    Type = PersonKind.Actor,
                    ItemId = Utils.GetGuidByName(name),
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
        protected override async Task<IList<SearchResult>> SearchAsync_impl(string keyword, CancellationToken cancellationToken) {
            var html = await GetHtmlAsync($"https://www.avbase.net/works?q={keyword}", cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var nodes = rootNode.SelectNodes("//div[@class='grow']/a");
            var results = new List<SearchResult>();
            if (nodes != null) {
                foreach (var node in nodes) {
                    var url = node.GetAttributeValue<string>("href", "");
                    results.Add(new SearchResult() { Id = url.Substring(url.LastIndexOf("/") + 1)});
                }
            }
            return results;
        }
        protected override async Task<string> GetMovieIdByName_impl(string name, string id, CancellationToken cancellationToken)
        {
            // if (!string.IsNullOrEmpty(id))
            //     return id;
            var searchResults = await SearchAsync(name, cancellationToken);
            if (searchResults.Count == 0)
                return "";
            return searchResults[0].Id;
        }
    }
}
