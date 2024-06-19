using Jellyfin.Data.Enums;
using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Controller.Entities;
using Microsoft.Extensions.Logging;
using System.Globalization;
namespace Jellyfin.Plugin.MyMetadata.Service.Test2 {
    public class TestHttpService : HttpService {
        public TestHttpService(ILogger<HttpService> logger, IHttpClientFactory http) : base(logger, http) { }
        protected override async Task<T> GetMovieAsync_impl<T>(string id, CancellationToken cancellationToken) {
            var url = $"https://jav5.land/tw/id_search.php?keys={id}";
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var item = new MovieItem();
            item.MovieId = id;
            item.Title = rootNode.SelectSingleNode("//span[@class='glyphicon glyphicon-film']")?.ParentNode?.InnerText;
            var imgNode = rootNode.SelectSingleNode("//div[@id='video_favorite_edit']");
            if (imgNode != null) {
                item.Fanart = imgNode.ParentNode.SelectSingleNode("img").GetAttributeValue<string>("src", "");
                item.Poster = item.Fanart;
            }
            var tableNode = rootNode.SelectSingleNode("//table[@class='videotextlist table table-bordered table-hover']");
            var infoNodes = tableNode.SelectNodes("tr");
            foreach (var node in infoNodes) {
                var nodes = node.SelectNodes("td");
                if (nodes == null || nodes.Count < 2) continue;
                var nameNode = nodes[0].SelectSingleNode("strong");
                if (nameNode == null) continue;
                var valueNode = nodes[1];
                if (valueNode == null) continue;
                logger.LogInformation($"解析信息 {nameNode.InnerText} = {valueNode.InnerText}");
                var key = nameNode.InnerText;
                if (key.Contains("發行日期")) {
                    if (DateTime.TryParseExact(valueNode.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                            item.ReleaseDate = releaseDate;
                } else if (key.Contains("系列")) {
                    foreach (var vNode in valueNode.SelectNodes("span/a")) {
                        item.Series.Add(vNode.InnerText);
                    }
                } else if (key.Contains("製作商")) {
                    foreach (var vNode in valueNode.SelectNodes("span/a")) {
                        item.Studios.Add(vNode.InnerText);
                    }
                } else if (key.Contains("發行商")) {
                    foreach (var vNode in valueNode.SelectNodes("span/a")) {
                        item.Labels.Add(vNode.InnerText);
                    }
                } else if (key.Contains("類別")) {
                    foreach (var vNode in valueNode.SelectNodes("span/a")) {
                        item.Genres.Add(vNode.InnerText);
                    }
                } else if (key.Contains("演員")) {
                    foreach (var vNode in valueNode.SelectNodes("span/span/span/a")) {
                        var name = vNode.InnerText;
                        item.Persons.Add(new PersonInfo() { 
                            Name = name,
                            Role = name,
                            Type = PersonKind.Actor,
                            ItemId = Utils.GetGuidByName(name)
                        });
                    }
                }
            }
            var shotscreensNodes = rootNode.SelectNodes("//span[@id='waterfall']/a");
            foreach (var node in shotscreensNodes) {
                item.Shotscreens.Add(node.GetAttributeValue<string>("href", ""));
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
    }
}
