using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Scorpio.Commons;
using System.Net;
using System.Net.Http.Headers;
using System;
public class HttpUtil {
    private const int READ_LENGTH = 8192;
    public class Response {
        public bool IsSuccessStatusCode { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public long Length { get; set; }
        public bool Skip { get; set; }
    }
    public static async Task<string> Get(string url, Action<HttpRequestMessage> preRequest = null) {
        using (var handler = new HttpClientHandler()) {
            handler.AllowAutoRedirect = false;
            handler.UseCookies = false;
            handler.Credentials = CredentialCache.DefaultCredentials;
            handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
            using (var client = new HttpClient(handler)) {
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
        using (var client = new HttpClient()) {
            try {
                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                var result = new Response() { StatusCode = response.StatusCode, IsSuccessStatusCode = response.IsSuccessStatusCode };
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
            } catch (System.Exception e) {
                return new Response() { StatusCode = 0, IsSuccessStatusCode = false };
            }
        } 
    }
}