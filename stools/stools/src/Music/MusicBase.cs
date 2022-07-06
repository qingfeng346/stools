using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Scorpio.Commons;
using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

public abstract class MusicBase {
    public abstract string Source { get; }
    /// <summary> 歌曲ID, 不同音乐平台的ID可能相同 </summary>
    public string ID { get; private set; }
    /// <summary> 歌曲名字 </summary>
    public string Name { get; protected set; }
    /// <summary> 专辑名字 </summary>
    public string Album { get; protected set; }
    public uint Year { get; protected set; }
    public uint Track { get; protected set; }
    /// <summary> 演唱者 </summary>
    public List<string> Singer { get; } = new List<string>();
    /// <summary> 封面图片地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> CoverUrls { get; } = new List<string>();
    /// <summary> mp3下载地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> Mp3Urls { get; } = new List<string>();

    //解析信息
    protected abstract Task ParseInfo(string id);

    //下载文件
    public async Task Download(string id, string path) {
        ID = id;
        Logger.info($"开始解析数据  来源:{Source} ID:{id}");
        for (var i = 0; i < 5; ++i) {
            try {
                await ParseInfo(id);
                break;
            } catch (Exception ex) {
                Logger.error($"解析数据出错,一秒后重试 {i+1}/3 : {ex}");
                Thread.Sleep(1000);
            }
        }
        await DownloadFile(path);
    }
    
    async Task DownloadFile(string savePath) {
        FileUtil.CreateDirectory(savePath);
        Logger.info("解析完成,开始下载 id:{0} 名字:{1}  歌手:{2}  专辑:{3}", ID, Name, Singer.GetSingers(), Album);
        var fileName = $"{Singer.GetSingers()} - {Name}.mp3";
        var filePath = Path.Combine(savePath, fileName);
        foreach (var mp3Url in Mp3Urls) {
            try {
                Logger.info($"尝试下载文件 : {mp3Url}");
                await HttpUtil.Download(mp3Url, filePath);
                break;
            } catch (Exception e) {
                Logger.error($"下载文件 {mp3Url} 失败 : {e}");
            }
        }
        if (!File.Exists(filePath)) { throw new Exception("音频文件下载失败"); }
        Logger.info("下载音频文件完成,文件大小:{1}", fileName, new FileInfo(filePath).Length.GetMemory());
        var file = TagLib.File.Create(filePath);
        file.Tag.Title = Name;
        file.Tag.Performers = Singer.ToArray();
        file.Tag.Album = Album;
        file.Tag.Year = Year;
        file.Tag.Track = Track;
        foreach (var coverUrl in CoverUrls) {
            try {
                var imageName = $"{Singer.GetSingers()} - {Name}.jpg";
                var imagePath = Path.Combine(savePath, imageName);
                Logger.info($"尝试下载封面 : {coverUrl}");
                await HttpUtil.Download(coverUrl, imagePath);
                ResizeImage(imagePath, 512);
                file.Tag.Pictures = new TagLib.IPicture[] { new TagLib.Picture(imagePath) };
                FileUtil.DeleteFile(imagePath);
                break;
            } catch (Exception e) {
                Logger.error($"下载封面 {coverUrl} 失败 : {e}");
            }
        }
        file.Save();
        var old = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Green;
        Logger.info("下载音乐完成 文件名:{0}  文件大小:{1}", Path.GetFullPath(filePath), new FileInfo(filePath).Length.GetMemory());
        Console.ForegroundColor = old;
        Logger.info("-------------------------------------------------------------------");
    }
    //重置封面大小
    void ResizeImage(string filePath, int size) {
        using (Image<Rgba32> image = Image.Load<Rgba32>(filePath)) {
            image.Mutate(x => x
                .Resize(size, size)
                .Grayscale());
            image.Save(filePath); // Automatic encoder selected based on extension.
        }
    }
}
