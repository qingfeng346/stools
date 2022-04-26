﻿using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Scorpio.Commons;
using System.Net;
using System;
public class HttpUtil {
    private const int READ_LENGTH = 8192;
    public static async Task<string> Get(string url, Action<HttpRequestMessage> preRequest = null) {
        var handler = new HttpClientHandler();
        handler.AllowAutoRedirect = true;
        handler.UseCookies = false;
        handler.Credentials = CredentialCache.DefaultCredentials;
        handler.ClientCertificateOptions = ClientCertificateOption.Automatic;
        var client = new HttpClient(handler);
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        //message.Headers.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        preRequest?.Invoke(message);
#if DEBUG
        Console.WriteLine($"Request Url : {url}");
#endif
        var response = await client.SendAsync(message);
        return await response.Content.ReadAsStringAsync();
    }
    public static async Task Download(string url, string file) {
        var client = new HttpClient();
        var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        var bytes = new byte[READ_LENGTH];
        using (var responseStream = await response.Content.ReadAsStreamAsync()) {
            long length = (response.Content.Headers?.ContentLength ?? 0);
            long readed = 0;
            long update = DateTime.UtcNow.Ticks;
            FileUtil.DeleteFile(file);
            using (var fileStream = new FileStream(file, FileMode.Create)) {
                while (true) {
                    var size = await responseStream.ReadAsync(bytes, 0, READ_LENGTH);
                    if (size <= 0) { break; }
                    readed += size;
                    fileStream.Write(bytes, 0, size);
                    long now = DateTime.UtcNow.Ticks;
                    if (now - update > 20000000) {
                        update = now;
                        Console.WriteLine($"{file} 进度: {readed.GetMemory()}/{length.GetMemory()}");
                    }
                }
            }
        }
    }
}