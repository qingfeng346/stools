using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Scorpio.Commons;
using System.Net;
using System.Net.Http.Headers;
using System;
using System.Text;
using System.Security.Authentication;
using RestSharp;

public class HttpUtil {
    private const int READ_LENGTH = 8192;
    private const int MaxRetryCount = 5;
    public class Response {
        public bool IsSuccessStatusCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Url { get; set; }
        public long Length { get; set; }
        public bool Skip { get; set; }
        public string Error { get; set; }
    }
    static HttpUtil() {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                               SecurityProtocolType.Tls11 |
                                               SecurityProtocolType.Tls12 |
                                               SecurityProtocolType.Tls13;
        // 忽略证书错误（仅用于调试）
        ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
        ServicePointManager.Expect100Continue = false;
    }
    public static async Task<string> Get(string url, Action<HttpRequestMessage> preRequest = null) {
        //var index = url.IndexOf("/", 8);
        //var baseUrl = url.Substring(0, index);
        //var requestUrl = url.Substring(index + 1);
        //var client = new RestClient(baseUrl);
        //var request = new RestRequest(requestUrl);
        //var result = await client.GetAsync(request);
        //return result.Content;
        using (var handler = new HttpClientHandler()) {
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.UseDefaultCredentials = true;
            handler.CookieContainer = new CookieContainer();
            handler.AutomaticDecompression = DecompressionMethods.All;
            handler.PreAuthenticate = false;
            handler.AllowAutoRedirect = true;
            //handler.UseCookies = false;
            //handler.Credentials = CredentialCache.DefaultCredentials;
            //handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            using (var client = new HttpClient(handler)) {
                client.Timeout = new TimeSpan(0, 0, 15);
                var cacheControl = new CacheControlHeaderValue();
                cacheControl.NoCache = true;
                cacheControl.NoStore = true;
                client.DefaultRequestHeaders.CacheControl = cacheControl;
                var message = new HttpRequestMessage(HttpMethod.Get, url);
                preRequest?.Invoke(message);
#if DEBUG
                Console.WriteLine($"Request Url : {url}");
#endif
                var response = await client.SendAsync(message);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
    public static async Task<Response> Download(string url, string file) {
        return await Download(url, file, true, false);
    }
    public static async Task<Response> Download(string url, string file, bool progress, bool checkSize) {
        for (int i = 0; i < MaxRetryCount; i++) {
            try {
                return await Download_impl(url, file, progress, checkSize);
            } catch (HttpRequestException ex) {
                if (i == MaxRetryCount - 1) {
                    throw; // 最后一次重试仍然失败，则抛出异常
                }
                // 等待一段时间后重试
                await Task.Delay(2000);
            }
        }
        throw new Exception("超过了最大重试次数");
    }
    //private static async Task<Response> Download_impl(string url, string file, bool progress, bool checkSize) {
    //    var options = new RestClientOptions();
    //    using (var client = new RestClient(options)) {
    //        try {
    //            FileUtil.DeleteFile(file);
    //            FileUtil.CreateDirectoryByFile(file);
    //            using (var fileStream = new FileStream(file, FileMode.Create)) {
    //                var req = new RestRequest(url, Method.Get);
    //                req.ResponseWriter = stream => {
    //                    using (stream) {
    //                        stream.CopyTo(fileStream);
    //                    }
    //                    return fileStream;
    //                };
    //                await client.DownloadDataAsync(req);
    //            }
    //            return new Response() { Url = url, StatusCode = 0, IsSuccessStatusCode = true, Error = "" }; ;
    //            //var cacheControl = new CacheControlHeaderValue();
    //            //cacheControl.NoCache = true;
    //            //cacheControl.NoStore = true;
    //            //client.DefaultRequestHeaders.CacheControl = cacheControl;
    //            //var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
    //            //var result = new Response() { Url = url, StatusCode = response.StatusCode, IsSuccessStatusCode = response.IsSuccessStatusCode };
    //            //if (!response.IsSuccessStatusCode) { return result; }
    //            //var bytes = new byte[READ_LENGTH];
    //            //using (var responseStream = await response.Content.ReadAsStreamAsync()) {
    //            //    long? contentLength = response.Content.Headers?.ContentLength;
    //            //    long readed = 0;
    //            //    long update = DateTime.UtcNow.Ticks;
    //            //    if (checkSize && contentLength != null && File.Exists(file) && new FileInfo(file).Length == contentLength) {
    //            //        result.Skip = true;
    //            //        result.Length = contentLength ?? 0;
    //            //        return result;
    //            //    }
    //            //    long length = contentLength ?? 0;
    //            //    FileUtil.DeleteFile(file);
    //            //    FileUtil.CreateDirectoryByFile(file);
    //            //    using (var fileStream = new FileStream(file, FileMode.Create)) {
    //            //        while (true) {
    //            //            var size = await responseStream.ReadAsync(bytes, 0, READ_LENGTH);
    //            //            if (size <= 0) { break; }
    //            //            readed += size;
    //            //            fileStream.Write(bytes, 0, size);
    //            //            long now = DateTime.UtcNow.Ticks;
    //            //            if (now - update > 20000000 && progress) {
    //            //                update = now;
    //            //                Console.WriteLine($"{file} 进度: {readed.GetMemory()}/{length.GetMemory()}");
    //            //            }
    //            //        }
    //            //    }
    //            //    result.Skip = false;
    //            //    result.Length = readed;
    //            //    return result;
    //            //}
    //        } catch (HttpRequestException ex) {
    //            var builder = new StringBuilder();
    //            if (ex.InnerException != null) {
    //                builder.AppendLine($"内层异常: {ex.InnerException.Message}");
    //                if (ex.InnerException is System.Net.Sockets.SocketException socketException) {
    //                    builder.AppendLine($"Socket 异常代码: {socketException.SocketErrorCode}");
    //                }
    //            }
    //            builder.AppendLine($"请求错误: {ex.ToString()}");
    //            return new Response() { Url = url, StatusCode = 0, IsSuccessStatusCode = false, Error = builder.ToString() };
    //        } catch (System.Exception e) {
    //            return new Response() { Url = url, StatusCode = 0, IsSuccessStatusCode = false, Error = e.ToString() };
    //        }
    //    }
    //}
    private static async Task<Response> Download_impl(string url, string file, bool progress, bool checkSize) {
        using (var handler = new HttpClientHandler()) {
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.UseDefaultCredentials = true;
            handler.CookieContainer = new CookieContainer();
            handler.AutomaticDecompression = DecompressionMethods.All;
            handler.PreAuthenticate = false;
            handler.AllowAutoRedirect = true;
            handler.SslProtocols = SslProtocols.Ssl2 |
                                   SslProtocols.Ssl3 |
                                   SslProtocols.Tls |
                                   SslProtocols.Tls11 |
                                   SslProtocols.Tls12 |
                                   SslProtocols.Tls13;
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient(handler)) {
                try {
                    var cacheControl = new CacheControlHeaderValue();
                    cacheControl.NoCache = true;
                    cacheControl.NoStore = true;
                    client.DefaultRequestHeaders.CacheControl = cacheControl;
                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    var result = new Response() { Url = url, StatusCode = response.StatusCode, IsSuccessStatusCode = response.IsSuccessStatusCode };
                    if (!response.IsSuccessStatusCode) { return result; }
                    var bytes = new byte[READ_LENGTH];
                    using (var responseStream = await response.Content.ReadAsStreamAsync()) {
                        long? contentLength = response.Content.Headers?.ContentLength;
                        long readed = 0;
                        long update = DateTime.UtcNow.Ticks;
                        if (checkSize && contentLength != null && File.Exists(file) && new FileInfo(file).Length == contentLength) {
                            result.Skip = true;
                            result.Length = contentLength ?? 0;
                            return result;
                        }
                        long length = contentLength ?? 0;
                        FileUtil.DeleteFile(file);
                        FileUtil.CreateDirectoryByFile(file);
                        using (var fileStream = new FileStream(file, FileMode.Create)) {
                            while (true) {
                                var size = await responseStream.ReadAsync(bytes, 0, READ_LENGTH);
                                if (size <= 0) { break; }
                                readed += size;
                                fileStream.Write(bytes, 0, size);
                                long now = DateTime.UtcNow.Ticks;
                                if (now - update > 20000000 && progress) {
                                    update = now;
                                    Console.WriteLine($"{file} 进度: {readed.GetMemory()}/{length.GetMemory()}");
                                }
                            }
                        }
                        result.Skip = false;
                        result.Length = readed;
                        return result;
                    }
                } catch (HttpRequestException ex) {
                    var builder = new StringBuilder();
                    if (ex.InnerException != null) {
                        builder.AppendLine($"内层异常: {ex.InnerException.Message}");
                        if (ex.InnerException is System.Net.Sockets.SocketException socketException) {
                            builder.AppendLine($"Socket 异常代码: {socketException.SocketErrorCode}");
                        }
                    }
                    builder.AppendLine($"请求错误: {ex.ToString()}");
                    return new Response() { Url = url, StatusCode = 0, IsSuccessStatusCode = false, Error = builder.ToString() };
                } catch (System.Exception e) {
                    return new Response() { Url = url, StatusCode = 0, IsSuccessStatusCode = false, Error = e.ToString() };
                }
            }
        }
    }
}