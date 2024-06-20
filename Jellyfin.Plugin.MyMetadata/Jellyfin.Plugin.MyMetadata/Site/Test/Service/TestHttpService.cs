using Jellyfin.Plugin.MyMetadata.Dto;
using Microsoft.Extensions.Logging;
using System.Globalization;
using WMJson;
namespace Jellyfin.Plugin.MyMetadata.Service.Test {
    public class TestHttpService : HttpService {
        public class MovieJsonData {
            public class Actor {
                public string name;
                public string image_url;
            }
            public class Cast {
                public Actor actor;
            }
            public class Genre {
                public string name;
            }
            public class Maker {
                public string name;
            }
            public class Label {
                public string name;
            }
            public class Serie {
                public string name;
            }
            public class SampleImage {
                public string s;
                public string l;
            }
            public class ItemInfo {
                public string volume;
            }
            public class Product {
                public string url;
                public string image_url;
                public string title;
                public string thumbnail_url;
                public Maker maker;
                public Label label;
                public Serie series;
                public List<SampleImage> sample_image_urls = new List<SampleImage>();
                public ItemInfo iteminfo;
            }
            public class Work {
                public string work_id;
                public string title;
                public string min_date;
                public string note = "";
                public List<Cast> casts = new List<Cast>();
                public List<string> tags = new List<string>();
                public List<Genre> genres = new List<Genre>();
                public List<Product> products = new List<Product>();
            }
            public class PageProps {
                public Work work;
            }
            public class Props {
                public PageProps pageProps;
            }
            public Props props;
        }
        public class PersonJsonData {
            public class Fanza {
                public string birthday;
                public string height;
                public string cup;
                public string prefectures;
            }
            public class Meta {
                public Fanza fanza;
            }
            public class Primary {
                public string name;
                public string image_url;
                public Meta meta;
            }
            public class Talent {
                public Primary primary;
            }
            public class PageProps {
                public Talent talent;
            }
            public class Props {
                public PageProps pageProps;
            }
            public Props props;
        }
        public TestHttpService(ILogger<HttpService> logger, IHttpClientFactory http) : base(logger, http) { }
        protected override async Task<T> GetMovieAsync_impl<T>(string id, CancellationToken cancellationToken) {
            var url = $"https://www.avbase.net/works/{id}";
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var jsonContent = rootNode.SelectSingleNode("//script[@id='__NEXT_DATA__']")?.InnerText;
            var movieInfo = JsonConvert.Deserialize<MovieJsonData>(jsonContent).props.pageProps.work;
            var item = new MovieItem();
            item.MovieId = movieInfo.work_id;
            item.Title = movieInfo.title;
            if (DateTime.TryParseExact(movieInfo.min_date, "ddd MMM dd yyyy HH:mm:ss 'GMT'zzz '('zzz')", CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate))
                item.ReleaseDate = releaseDate;
            movieInfo.genres.ForEach(x => item.Genres.Add(x.name));
            movieInfo.casts.ForEach(x => {
                var personItem = new PersonItem() {
                    Name = x.actor.name,
                    ImageUrl = x.actor.image_url,
                    PersonId = x.actor.name
                };
                item.Persons.Add(personItem);
            });
            var product = movieInfo.products.First();
            item.ImageUrl = product.image_url;
            item.ThumbUrl = product.thumbnail_url;
            if (product.maker != null)
                item.Studios.Add(product.maker.name);
            if (product.label != null)
                item.Labels.Add(product.label.name);
            if (product.series != null)
                item.Series.Add(product.series.name);
            product.sample_image_urls.ForEach(x => {
                item.Shotscreens.Add(x.l);
            });
            item.SourceUrl = url;
            return item as T;
        }
        protected override async Task<T> GetPersonAsync_impl<T>(string id, CancellationToken cancellationToken) {
            var url = $"https://www.avbase.net/talents/{id}";
            var html = await GetHtmlAsync(url, cancellationToken);
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            var rootNode = doc.DocumentNode;
            var jsonContent = rootNode.SelectSingleNode("//script[@id='__NEXT_DATA__']")?.InnerText;
            var personInfo = JsonConvert.Deserialize<PersonJsonData>(jsonContent).props.pageProps.talent;
            var item = new PersonItem();
            item.Name = personInfo.primary.name;
            item.ImageUrl = personInfo.primary.image_url;
            if (personInfo.primary.meta != null) {
                if (DateTime.TryParseExact(personInfo.primary.meta.fanza.birthday, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    item.PremiereDate = date;
                item.Desc = @$"出生地:{personInfo.primary.meta.fanza.prefectures}
    身高:{personInfo.primary.meta.fanza.height}cm
    罩杯:{personInfo.primary.meta.fanza.cup}";
            }
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
            var searchResults = await SearchAsync(name, cancellationToken);
            if (searchResults.Count == 0)
                return "";
            return searchResults[0].Id;
        }
    }
}
