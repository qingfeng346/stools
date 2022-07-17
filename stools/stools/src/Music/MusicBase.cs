using System.Collections.Generic;
using System.Threading.Tasks;
using Scorpio.Commons;
using System;
using System.IO;
using TagLib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

public abstract class MusicBase {
    public const int RetryTotal = 10;
    public class AlbumInfo {
        public string name;
        public List<string> musicList;
    }
    public abstract string Source { get; }
    /// <summary> 歌曲ID, 不同音乐平台的ID可能相同 </summary>
    public string ID { get; private set; }
    /// <summary> 歌曲名字 </summary>
    public string Name { get; protected set; }
    /// <summary> 专辑名字 </summary>
    public string Album { get; protected set; }
    /// <summary> 年份 </summary>
    public uint Year { get; protected set; }
    /// <summary> Track </summary>
    public uint Track { get; protected set; }
    /// <summary> 歌词 </summary>
    public string Lyrics { get; protected set; } = "";
    /// <summary> 演唱者 </summary>
    public List<string> Singer { get; } = new List<string>();
    /// <summary> 封面图片地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> CoverUrls { get; } = new List<string>();
    /// <summary> mp3下载地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> Mp3Urls { get; } = new List<string>();

    //下载文件
    public async Task Download(string id, string path) {
        ID = id;
        Logger.info($"开始解析数据  来源:{Source} ID:{id}");
        Name = "";
        Album = "";
        Year = 0;
        Track = 0;
        var success = false;
        for (var i = 0; i < RetryTotal; ++i) {
            try {
                Singer.Clear();
                CoverUrls.Clear();
                Mp3Urls.Clear();
                await ParseInfo(id);
                success = true;
                break;
            } catch (Exception ex) {
                Logger.error($"解析数据出错,一秒后重试 {i + 1}/{RetryTotal} : {ex}");
                await Task.Delay(1000);
            }
        }
        if (!success) {
            throw new Exception("解析音乐数据出错");
        }
        await DownloadFile(path);
    }
    public async Task<AlbumInfo> ParseAlbum(string id) {
        for (var i = 0; i < RetryTotal; ++i) {
            try {
                return await ParseAlbum_impl(id);
            } catch (Exception ex) {
                Logger.error($"解析专辑出错,一秒后重试 {i + 1}/{RetryTotal} : {ex}");
                await Task.Delay(1000);
            }
        }
        throw new Exception("解析专辑信息出错");
    }
    //解析信息
    protected abstract Task ParseInfo(string id);
    //解析专辑
    protected abstract Task<AlbumInfo> ParseAlbum_impl(string id);
    async Task DownloadFile(string savePath) {
        FileUtil.CreateDirectory(savePath);
        Logger.info("解析完成,开始下载 id:{0} 名字:{1}  歌手:{2}  专辑:{3}  年份:{4}", ID, Name, Singer.GetSingers(), Album, Year);
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
        if (!System.IO.File.Exists(filePath)) { throw new Exception("音频文件下载失败"); }
        Logger.info("下载音频文件完成,文件大小:{1}", fileName, new FileInfo(filePath).Length.GetMemory());
        var file = TagLib.File.Create(filePath);
        file.Tag.Title = Name;
        file.Tag.Performers = Singer.ToArray();
        file.Tag.Album = Album;
        file.Tag.Year = Year;
        file.Tag.Track = Track;
        file.Tag.Publisher = $"{Source} - {ID}";
        file.Tag.Lyrics = Lyrics;
        file.Tag.Genres = new string[0];
        foreach (var coverUrl in CoverUrls) {
            try {
                //var imagePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
                var imagePath = Path.Combine(savePath, $"{Guid.NewGuid()}.png");
                Logger.info($"尝试下载封面 : {coverUrl}");
                await HttpUtil.Download(coverUrl, imagePath);
                ResizeImage(imagePath, 512);
                var pictures = new List<IPicture>();
                {
                    var picture = new TagLib.Id3v2.AttachmentFrame(new Picture(imagePath));
                    picture.Type = PictureType.FrontCover;
                    picture.Filename = "frontcover.png";
                    picture.Description = "";
                    picture.MimeType = Picture.GetMimeFromExtension(imagePath);
                    picture.TextEncoding = StringType.Latin1;
                    //var picture = new Picture(imagePath);
                    //picture.Type = PictureType.FrontCover;
                    //picture.Filename = "frontcover.png";
                    //picture.Description = "";
                    pictures.Add(picture);
                }
                file.Tag.Pictures = pictures.ToArray();
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
            image.Mutate(x => x.Resize(size, size));
            image.Save(filePath); // Automatic encoder selected based on extension.
        }
    }
}
