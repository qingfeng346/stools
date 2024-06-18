using Jellyfin.Data.Enums;
using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Jellyfin.Plugin.MyMetadata.Service.Test2 {
    public class TestHttpService : HttpService
    {
        public TestHttpService(ILogger<HttpService> logger, IHttpClientFactory http) : base(logger, http) { }
        public async Task<T> GetMovieAsync<T>(string id, CancellationToken cancellationToken) where T : MovieItem
        {
            var html = await GetHtmlAsync(id, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var item = new MovieItem();
            item.Id = id.Substring(id.LastIndexOf(":") + 1);
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
            item.SourceUrl = id;
            return item as T;
        }

        public override async Task<MetadataResult<Movie>> GetMovieMetadataAsync(string id, CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            var result = new MetadataResult<Movie>();
            //// 获取影片详情
            var item = await GetMovieAsync<MovieItem>(id, cancellationToken);
            // 设置 基础信息
            var movie = new Movie {
                Name = $"{item.Id} {item.Title}",
                OriginalTitle = item.Title,
                HomePageUrl = id,
            };
            // 如果 系列 不为空
            if (item.Series?.Count > 0) {
               movie.CollectionName = item.Series?[0];
            }
            // 如果 发行日期 不为空
            if (item.ReleaseDate != null)
            {
               var releaseDate = item.ReleaseDate?.ToUniversalTime();
               // 设置 发行日期
               movie.PremiereDate = releaseDate;
               // 设置 年份
               movie.ProductionYear = releaseDate?.Year;
            }
            // 添加类别
            item.Genres?.ForEach(movie.AddGenre);
            // 添加 工作室
            item.Studios?.ForEach(movie.AddStudio);
            // 添加 发行商
            item.Labels?.ForEach(movie.AddStudio);
            // 演员
            item.Persons.ForEach(result.AddPerson);
            result.QueriedById = false;
            result.HasMetadata = true;
            result.Item = movie;
            return result;
        }

        public async Task<IEnumerable<SearchResult>> SearchAsync(string keyword, CancellationToken cancellationToken) {
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
