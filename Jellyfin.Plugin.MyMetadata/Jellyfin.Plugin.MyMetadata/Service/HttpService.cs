using System.Net.Security;
using Jellyfin.Data.Entities;
using Jellyfin.Plugin.MyMetadata.Dto;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using Microsoft.Extensions.Logging;
using WMJson;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public abstract class HttpService {
        protected readonly ILogger<HttpService> logger;
        protected readonly IHttpClientFactory http;
        protected readonly List<(string, MovieItem)> cacheMovies;
        protected readonly List<(string, PersonItem)> cachePersions;
        public HttpService(ILogger<HttpService> logger, IHttpClientFactory http) {
            this.logger = logger;
            this.http = http;
            this.cacheMovies = new List<(string, MovieItem)>();
            this.cachePersions = new List<(string, PersonItem)>();
        }
        /// <summary>获取响应对象 </summary>
        public async Task<HttpResponseMessage> GetResponseAsync(string url, CancellationToken cancellationToken) {
            logger.LogInformation($"GetResponseAsync : {url}");
            using (var httpClientHandler = new HttpClientHandler()) {
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                    if (sslPolicyErrors == SslPolicyErrors.None) {
                        return true;   //Is valid
                    }
                    return true;
                };
                using (var client = new HttpClient(httpClientHandler)) {
                    return await client.GetAsync(url, cancellationToken);
                }
            }
            // return await http.CreateClient(NamedClient.Default).GetAsync(url, cancellationToken);
        } 
        /// <summary>下载 HTMl 源码 </summary>
        public async Task<string> GetHtmlAsync(string url, CancellationToken cancellationToken) {
            logger.LogInformation($"GetHtmlAsync : {url}");
            using (var httpClientHandler = new HttpClientHandler()) {
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => {
                    if (sslPolicyErrors == SslPolicyErrors.None) {
                        return true;   //Is valid
                    }
                    return true;
                };
                using (var client = new HttpClient(httpClientHandler)) {
                    return await client.GetStringAsync(url, cancellationToken);
                }
            }
            // return await http.CreateClient(NamedClient.Default).GetStringAsync(url, cancellationToken);
        }
        /// <summary> 获取影片元数据 </summary>
        public async Task<MetadataResult<Movie>> GetMovieMetadataAsync(string id, CancellationToken cancellationToken) {
            await Task.Delay(1);
            var result = new MetadataResult<Movie>();
            //// 获取影片详情
            var item = await GetMovieAsync<MovieItem>(id, cancellationToken);
            if (item == null) return result;
            // 设置 基础信息
            var movie = new Movie {
                Name = $"{item.MovieId} {item.Title}",
                OriginalTitle = item.Title,
                Overview = item.Title,
                HomePageUrl = id,
            };
            // 如果 系列 不为空
            if (item.Series.Count > 0)
                movie.CollectionName = item.Series[0];
            // 如果 发行日期 不为空
            if (item.ReleaseDate != null) {
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
            item.Persons.ForEach(x => result.AddPerson(x.ToPersonInfo()));
            // 系列
            movie.Tags = item.Series.ToArray();
            result.QueriedById = false;
            result.HasMetadata = true;
            result.Item = movie;
            return result;
        }
        /// <summary> 获取影片元数据 </summary>
        public async Task<T> GetMovieAsync<T>(string id, CancellationToken cancellationToken) where T : MovieItem {
            try {
                foreach (var (movieId, movieItem) in cacheMovies) {
                    if (movieId == id) {
                        logger.LogInformation($"GetMovieAsync {id} Result Cache : {JsonConvert.Serialize(movieItem)}");
                        return movieItem as T;
                    }
                }
                var item = await GetMovieAsync_impl<T>(id, cancellationToken);
                logger.LogInformation($"GetMovieAsync {id} Result : {JsonConvert.Serialize(item)}");
                cacheMovies.Add((id, item));
                if (cacheMovies.Count > 10)
                    cacheMovies.RemoveAt(0);
                return item;
            } catch (Exception e) {
                logger.LogError($"GetMovieAsync : {id} is error : {e}");
                return null;
            }
        }
        /// <summary> 获取演员元数据 </summary>
        public async Task<MetadataResult<Person>> GetPersonMetadataAsync(string id, CancellationToken cancellationToken) {
            await Task.Delay(1);
            var result = new MetadataResult<Person>();
            //// 获取影片详情
            var item = await GetPersonAsync<PersonItem>(id, cancellationToken);
            if (item == null) return result;
            var person = new Person();
            if (item.PremiereDate != null) {
                person.PremiereDate = item.PremiereDate;
                person.ProductionYear = item.PremiereDate.Value.Year;
            }
            person.Overview = item.Desc;
            result.Item = person;
            result.HasMetadata = true;
            return result;
        }
        /// <summary> 获取演员元数据 </summary>
        public async Task<T> GetPersonAsync<T>(string id, CancellationToken cancellationToken) where T : PersonItem {
            try {
                foreach (var (personId, personItem) in cachePersions) {
                    if (personId == id) {
                        logger.LogInformation($"GetPersonAsync {id} Result Cache : {JsonConvert.Serialize(personItem)}");
                        return personItem as T;
                    }
                }
                var item = await GetPersonAsync_impl<T>(id, cancellationToken);
                logger.LogInformation($"GetPersonAsync {id} Result : {JsonConvert.Serialize(item)}");
                cachePersions.Add((id, item));
                if (cachePersions.Count > 10)
                    cachePersions.RemoveAt(0);
                return item;
            } catch (Exception e) {
                logger.LogError($"GetPersonAsync : {id} is error : {e}");
                return null;
            }
        }
        public async Task<IList<SearchResult>> SearchAsync(string keyword, CancellationToken cancellationToken) {
            try {
                var results = await SearchAsync_impl(keyword, cancellationToken);
                logger.LogInformation($"SearchAsync {keyword} Result : {JsonConvert.Serialize(results)}");
                return results ?? [];
            } catch (Exception e) {
                logger.LogError($"SearchAsync : {keyword} is error : {e}");
                return [];
            }
        }
        public async Task<string> GetMovieIdByName(string name, string id, string path, CancellationToken cancellationToken) {
            try {
                var result = await GetMovieIdByName_impl(name, id, path, cancellationToken);
                logger.LogInformation($"GetMovieIdByName name:{name} id:{id} path:{path} Result : {result}");
                return result;
            } catch (Exception e) {
                logger.LogError($"GetMovieIdByName name:{name} id:{id} path:{path} is error : {e}");
                return "";
            }
        }
        public async Task<string> GetPersonIdByName(string id, string name, string path, CancellationToken cancellationToken) {
            try {
                var result = await GetPersonIdByName_impl(id, name, path, cancellationToken);
                logger.LogInformation($"GetPersonIdByName name:{name} id:{id} path:{path} Result : {result}");
                return result;
            } catch (Exception e) {
                logger.LogError($"GetPersonIdByName name:{name} id:{id} path:{path} is error : {e}");
                return "";
            }
        }
        protected abstract Task<T> GetMovieAsync_impl<T>(string id, CancellationToken cancellationToken) where T : MovieItem;
        protected abstract Task<IList<SearchResult>> SearchAsync_impl(string keyword, CancellationToken cancellationToken);
        protected abstract Task<T> GetPersonAsync_impl<T>(string id, CancellationToken cancellationToken) where T : PersonItem;
        protected virtual async Task<string> GetMovieIdByName_impl(string name, string id, string path, CancellationToken cancellationToken) {
            if (!string.IsNullOrEmpty(id))
                return id;
            return name;
        }
        protected virtual async Task<string> GetPersonIdByName_impl(string id, string name, string path, CancellationToken cancellationToken) {
            return name;
        }
    }
}
