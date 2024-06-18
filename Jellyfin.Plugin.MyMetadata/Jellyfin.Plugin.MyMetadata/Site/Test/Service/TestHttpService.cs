using Jellyfin.Data.Enums;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class TestHttpService : HttpService
    {
        protected readonly ILogger<HttpService> logger;
        public TestHttpService(ILogger<TestHttpService> logger, IHttpClientFactory http) : base(logger, http)
        {
            this.logger = logger;
        }


        //public override async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken)
        //{

        //    var request = new HttpRequestMessage(HttpMethod.Get, url);
        //    var response = await _http.CreateClient(NamedClient.Default).SendAsync(request);
        //    var content = await response.Content.ReadAsStringAsync();
        //    return content;
        //}
        //public override async Task<HttpResponseMessage> GetResponseAsync(string url, CancellationToken cancellationToken)
        //{
        //    var request = new HttpRequestMessage(HttpMethod.Get, url);
        //    var response = await _http.CreateClient(NamedClient.Default).SendAsync(request);
        //    var content = await response.Content.ReadAsStringAsync();
        //    return response;
        //}

        //public override async Task<T> GetMovieAsync<T>(string id, CancellationToken cancellationToken)
        //{
        //    // id 不能为空
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        _logger.LogError("id is null.");
        //        throw new ArgumentException("sid is empty when getting subject");
        //    }

        //    // 拼接 url
        //    //var url = $"https://{Plugin.Instance.Configuration.Url}/videos/?id={id}";

        //    // 确认
        //    //await ConfirmAsync();

        //    //// 拉取 html
        //    //var html = await GetHtmlAsync(url, cancellationToken);

        //    //var item = new MovieItem();

        //    //var fanart = await GetFanartAsync(html, cancellationToken);
        //    //item.Avid = await GetAvidAsync(html, cancellationToken);
        //    //item.Title = await GetTitleAsync(html, cancellationToken);
        //    //item.Poster = fanart.Replace("_l", "_m");
        //    //item.Fanart = fanart;
        //    //item.ReleaseDate = GetReleaseDate(html);
        //    //item.Duration = GetDuration(html);
        //    //item.Directors = await GetDirectorsAsync(html, cancellationToken);
        //    //item.Studios = await GetStudiosAsync(html, cancellationToken);
        //    //item.Labels = await GetLabelsAsync(html, cancellationToken);
        //    //item.Series = await GetSeriesAsync(html, cancellationToken);
        //    //item.Genres = await GetGenresAsync(html, cancellationToken);
        //    //item.Actresses = await GetActressesAsync(html, cancellationToken);
        //    //item.Shotscreens = await GetScreenshotsAsync(html, cancellationToken);
        //    //item.SourceUrl = url;
        //    //return item as T;
        //}

        public override async Task<MetadataResult<Movie>> GetMovieMetadataAsync(string id, CancellationToken cancellationToken)
        {
            await Task.Delay(1);
            var result = new MetadataResult<Movie>();
            logger.LogWarning($"GetMovieMetadataAsync : " + id);

            //// 如果 id 为空，则直接返回空结果
            //if (string.IsNullOrWhiteSpace(id))
            //{
            //    _logger.LogError("sid is empty when getting subject.");
            //    return result;
            //}

            //// 获取影片详情
            //var item = await GetMovieAsync<MovieItem>(id, cancellationToken);

            // 设置 基础信息
            var movie = new Movie {
                Name = "Name",
                OriginalTitle = "Title",
                SortName = "SortName",
                ForcedSortName = "ForcedSortName",
                Overview = "Overview",
                HomePageUrl = $"http://am3tools.diandian.info:6789/static/ClientAssets/plant_444dinosaur_stone01.png",
            };
            movie.AddGenre("genre1");
            movie.AddGenre("genre2");
            result.QueriedById = false;
            result.HasMetadata = true;
            result.Item = movie;
            return result;
            //// 如果 系列 不为空
            //if (item.Series?.Count > 0)
            //{
            //    // 设置合集名
            //    movie.CollectionName = item.Series?[0];
            //}

            //// 如果 发行日期 不为空
            //if (item.ReleaseDate != null)
            //{
            //    var releaseDate = item.ReleaseDate?.ToUniversalTime();

            //    // 设置 发行日期
            //    movie.PremiereDate = releaseDate;
            //    // 设置 年份
            //    movie.ProductionYear = releaseDate?.Year;
            //}

            //// 添加类别
            //item.Genres.ForEach((item) =>
            //{
            //    movie.AddGenre(item);
            //});

            //// 添加 工作室
            //item.Studios.ForEach((studio) =>
            //{
            //    movie.AddStudio(studio);
            //});

            //// 添加 发行商
            //item.Labels.ForEach((label) =>
            //{
            //    // 不存在时才添加
            //    if (!movie.Studios.Contains(label))
            //    {
            //        movie.AddStudio(label);
            //    }
            //});

            //// 添加 导演
            //(await TransPersonInfoAsync(item.Directors, PersonKind.Director, cancellationToken))?.ForEach((item) =>
            //{
            //    result.AddPerson(item);
            //});

            //// 添加 演员
            //(await TransPersonInfoAsync(item.Actresses, PersonKind.Actor, cancellationToken))?.ForEach((item) =>
            //{
            //    result.AddPerson(item);
            //});

            //// 添加 编剧
            ////await TransPersonInfoAsync(movie.Writers, PersonType.Writer, cancellationToken).ForEach(result.AddPerson);

            //result.QueriedById = false;
            //result.HasMetadata = true;
            //result.Item = movie;

            //return result;
        }

        //public override async Task<IEnumerable<T>> SearchAsync<T>(string keyword, CancellationToken cancellationToken)
        //{
        //    //var results = new List<T>();

        //    //// 拼接 url
        //    //var url = $"https://{Plugin.Instance.Configuration.Sod.Domain}/videos/genre/?search_type=1&sodsearch={keyword}";

        //    //// 确认
        //    ////await ConfirmAsync();

        //    //// 查询
        //    //var html = await GetHtmlAsync(url, cancellationToken);
        //    //// 匹配影片
        //    //var matches = Regex.Matches(html, Plugin.Instance.Configuration.Sod.SearchResultPattern);

        //    //foreach (Match match in matches)
        //    //{
        //    //    if (match.Success)
        //    //    {
        //    //        var item = new SearchResult { Id = match.Groups["avid"].Value.Trim() };
        //    //        results.Add(item as T);
        //    //    }
        //    //}
        //    //// 返回 30 条记录
        //    //return (IEnumerable<T>)results.Distinct().Take(30).ToList();
        //}
    }
}
