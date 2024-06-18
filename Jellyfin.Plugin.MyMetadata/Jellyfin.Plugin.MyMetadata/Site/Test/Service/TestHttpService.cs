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

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class TestHttpService : HttpService
    {
        public TestHttpService(ILogger<HttpService> logger, IHttpClientFactory http) : base(logger, http) { }
        public async Task<T> GetMovieAsync<T>(string url, CancellationToken cancellationToken) where T : MovieItem
        {
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var item = new MovieItem();
            item.Title = rootNode.SelectSingleNode("//h1[@class='text-lg']")?.InnerText;
            item.Fanart = rootNode.SelectSingleNode("//img[@class='max-h-full']").GetAttributeValue<string>("src", "");
            item.Poster = item.Fanart;
            var infoNodes = rootNode.SelectNodes("//div[@class='text-sm']/a[@class='link']");
            if (DateTime.TryParseExact(infoNodes[0].InnerText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                item.ReleaseDate = releaseDate;
            // item.Directors = new List<PersonModel>() {new PersonModel()};
            // item.ReleaseDate = infoNodes[0].InnerText;
            // item.ReleaseDate = infoNodes[0].InnerText;
            // item.Fanart = fanart;
            // item.ReleaseDate = GetReleaseDate(html);
            // item.Duration = GetDuration(html);
            // item.Directors = await GetDirectorsAsync(html, cancellationToken);
            // item.Studios = await GetStudiosAsync(html, cancellationToken);
            // item.Labels = await GetLabelsAsync(html, cancellationToken);
            // item.Series = await GetSeriesAsync(html, cancellationToken);
            // item.Genres = await GetGenresAsync(html, cancellationToken);
            // item.Actresses = await GetActressesAsync(html, cancellationToken);
            // item.Shotscreens = await GetScreenshotsAsync(html, cancellationToken);
            item.SourceUrl = url;
            return item as T;
        }

        public override async Task<MetadataResult<Movie>> GetMovieMetadataAsync(string url, CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            var result = new MetadataResult<Movie>();
            //// 获取影片详情
            var item = await GetMovieAsync<MovieItem>(url, cancellationToken);
            // 设置 基础信息
            var movie = new Movie {
                Name = item.Title,
                OriginalTitle = item.Title,
                HomePageUrl = url,
            };
            // 如果 系列 不为空
            if (item.Series?.Count > 0)
            {
               // 设置合集名
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
            item.Genres.ForEach(movie.AddGenre);
            // 添加 工作室
            item.Studios.ForEach(movie.AddStudio);
            // 添加 发行商
            item.Labels.ForEach((label) =>
            {
               // 不存在时才添加
               if (!movie.Studios.Contains(label))
               {
                   movie.AddStudio(label);
               }
            });
            //// 添加 导演
            // (await TransPersonInfoAsync(item.Directors, PersonKind.Director, cancellationToken))?.ForEach((item) =>
            // {
            //    result.AddPerson(item);
            // });

            //// 添加 演员
            //(await TransPersonInfoAsync(item.Actresses, PersonKind.Actor, cancellationToken))?.ForEach((item) =>
            //{
            //    result.AddPerson(item);
            //});

            //// 添加 编剧
            ////await TransPersonInfoAsync(movie.Writers, PersonType.Writer, cancellationToken).ForEach(result.AddPerson);
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
            var nodes = rootNode.SelectNodes("//a[@class='text-md']");
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
