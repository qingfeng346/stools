using Jellyfin.Plugin.MyMetadata.Dto;
using Microsoft.Extensions.Logging;
using System.Globalization;
namespace Jellyfin.Plugin.MyMetadata.Service.Test3 {
    public class TestHttpService : HttpService {
        public TestHttpService(ILogger<HttpService> logger, IHttpClientFactory http) : base(logger, http) { }
        protected override async Task<T> GetMovieAsync_impl<T>(string id, CancellationToken cancellationToken) {
            var rootUrl = "https://www.busjav.cfd";
            var url = $"{rootUrl}/{id}";
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var item = new MovieItem();
            item.MovieId = id;
            item.Title = rootNode.SelectSingleNode("//div[@class='container']/h3")?.InnerText;
            var imgNode = rootNode.SelectSingleNode("//a[@class='bigImage']");
            if (imgNode != null) {
                item.ImageUrl = rootUrl + imgNode.GetAttributeValue<string>("href", "");
                item.ThumbUrl = item.ImageUrl;
            }
            var infoNodes = rootNode.SelectNodes("//div[@class='col-md-3 info']");
            foreach (var node in infoNodes) {
                var nameNode = node.SelectSingleNode("span");
                if (nameNode == null) continue;
                logger.LogInformation($"解析信息 {nameNode.InnerText} = {node.InnerText}");
                var key = nameNode.InnerText;
                if (key.Contains("發行日期")) {
                    if (DateTime.TryParseExact(node.InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                            item.ReleaseDate = releaseDate;
                } else if (key.Contains("系列")) {
                    item.Series.Add(node.SelectSingleNode("a").InnerText);
                } else if (key.Contains("製作商")) {
                    item.Studios.Add(node.SelectSingleNode("a").InnerText);
                } else if (key.Contains("發行商")) {
                    item.Labels.Add(node.SelectSingleNode("a").InnerText);
                }
            }
            var genreNodes = rootNode.SelectNodes("//span[@class='genre']/label/a");
            foreach (var node in genreNodes) {
                item.Genres.Add(node.InnerText);
            }
            var actorNodes = rootNode.SelectNodes("//div[@class='star-name']");
            foreach (var node in actorNodes) {
                item.Persons.Add(new PersonItem() {
                    Name = node.InnerText,
                });
            }
            // var shotscreensNodes = rootNode.SelectNodes("//span[@id='waterfall']/a");
            // foreach (var node in shotscreensNodes) {
            //     item.Shotscreens.Add(node.GetAttributeValue<string>("href", ""));
            // }
            item.SourceUrl = url;
            return item as T;
        }
        protected override Task<T> GetPersonAsync_impl<T>(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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
