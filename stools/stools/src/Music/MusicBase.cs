﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Scorpio.Commons;
using System;
using System.IO;
using TagLib;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

public enum MusicPath {
    None = 0,               //不创建文件夹
    Artist = 1,             //创建歌手文件夹
    Album = 2,              //创建专辑文件夹
    ArtistAlbum = 3,        //创建歌手专辑两层文件夹
}
public abstract class MusicBase {
    public const int RetryTotal = 10;
    public class AlbumInfo {
        public string name;                 //专辑名字
        public string artist;               //演唱者
        public List<string> musicList = new List<string>();      //专辑歌曲列表
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
    public string FilePath { get; protected set; } = "";
    public long Duration { get; protected set; } = 0;
    /// <summary> 演唱者 </summary>
    public List<string> Singer { get; } = new List<string>();
    /// <summary> 封面图片地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> CoverUrls { get; } = new List<string>();
    /// <summary> mp3下载地址,可能有多个地址,顺序尝试下载 </summary>
    public List<string> Mp3Urls { get; } = new List<string>();

    //下载文件
    public async Task<bool> Download(string id, string path, MusicPath musicPath) {
        ID = id;
        logger.info($"==============开始解析数据,来源:{Source} ID:{id}==============");
        for (var i = 0; i < RetryTotal; ++i) {
            try {
                Name = "";
                Album = "";
                Year = 0;
                Track = 0;
                Singer.Clear();
                CoverUrls.Clear();
                Mp3Urls.Clear();
                await ParseInfo(id);
                return await DownloadFile(path, musicPath);
            } catch (Exception ex) {
                logger.error($"下载音乐【{Name}】失败,3秒后重试 {i + 1}/{RetryTotal} : {ex}");
                await Task.Delay(3000);
            }
        }
        return false;
    }
    public async Task<AlbumInfo> ParseAlbum(string id) {
        for (var i = 0; i < RetryTotal; ++i) {
            try {
                var albumInfo = await ParseAlbum_impl(id);
                using (var log = new LoggerColor(ConsoleColor.Green))
                    logger.info($"解析专辑完成, 名称:{albumInfo.name}  歌手:{albumInfo.artist}  歌曲数量:{albumInfo.musicList.Count}");
                return albumInfo;
            } catch (Exception ex) {
                logger.error($"解析专辑出错,3秒后重试 {i + 1}/{RetryTotal} : {ex}");
                await Task.Delay(3000);
            }
        }
        throw new Exception("解析专辑信息出错");
    }
    //解析信息
    protected abstract Task ParseInfo(string id);
    //解析专辑
    protected abstract Task<AlbumInfo> ParseAlbum_impl(string id);
    async Task<bool> DownloadFile(string savePath, MusicPath musicPath) {
        try {
            logger.info("解析完成,开始下载 id:{0} 名字:{1}  歌手:{2}  专辑:{3}  年份:{4}", ID, Name, Singer.GetSingers(), Album, Year);
            var fileName = $"{Singer.GetSingers()} - {Name}.mp3";
            FilePath = musicPath switch {
                MusicPath.Artist => Path.Combine(savePath, Singer.GetSingers(), fileName),
                MusicPath.Album => Path.Combine(savePath, Album, fileName),
                MusicPath.ArtistAlbum => Path.Combine(savePath, Singer.GetSingers(), Album, fileName),
                _ => Path.Combine(savePath, fileName),
            };
            FilePath = Path.GetFullPath(FilePath);
            FileUtil.CreateDirectoryByFile(FilePath);
            foreach (var mp3Url in Mp3Urls) {
                try {
                    logger.info($"尝试下载文件 : {mp3Url}");
                    await HttpUtil.Download(mp3Url, FilePath);
                    break;
                } catch (Exception e) {
                    logger.error($"下载文件 {mp3Url} 失败 : {e}");
                }
            }
            if (!System.IO.File.Exists(FilePath)) { throw new Exception("音频文件下载失败"); }
            logger.info("下载音频文件完成,文件大小:{1}", fileName, new FileInfo(FilePath).Length.GetMemory());
            var file = TagLib.File.Create(FilePath);
            file.Tag.Title = Name;
            file.Tag.Performers = Singer.ToArray();
            file.Tag.Album = Album;
            file.Tag.Year = Year;
            file.Tag.Track = Track;
            file.Tag.Publisher = $"{Source} - {ID}";
            file.Tag.Lyrics = Lyrics;
            file.Tag.Genres = new string[0];
            Duration = (long)file.Properties.Duration.TotalMilliseconds;
            foreach (var coverUrl in CoverUrls) {
                string imagePath = "";
                try {
                    imagePath = Path.Combine(savePath, $"{Guid.NewGuid()}.png");
                    logger.info($"尝试下载封面 : {coverUrl}");
                    await HttpUtil.Download(coverUrl, imagePath);
                    //重置封面大小
                    using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath)) {
                        image.Mutate(x => x.Resize(512, 512));
                        image.Save(imagePath);
                    }
                    var pictures = new List<IPicture>();
                    {
                        var picture = new TagLib.Id3v2.AttachmentFrame(new Picture(imagePath)) {
                            Type = PictureType.FrontCover,
                            Filename = "frontcover.png",
                            Description = "",
                            MimeType = Picture.GetMimeFromExtension(imagePath),
                            TextEncoding = StringType.Latin1
                        };
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
                    FileUtil.DeleteFile(imagePath);
                    logger.error($"下载封面 {coverUrl} 失败 : {e}");
                }
            }
            file.Save();
            using (var log = new LoggerColor(ConsoleColor.Green)) {
                logger.info("下载音乐完成 文件名:{0}  文件大小:{1}  时长:{2}", Path.GetFullPath(FilePath), new FileInfo(FilePath).Length.GetMemory(), GetDuration(Duration));
            }
            return true;
        } catch (Exception ) {
            FileUtil.DeleteFile(FilePath);
            throw;
        }
    }
    string GetDuration(long duration) {
        var seconds = duration / 1000;
        if (seconds >= 60) {
            return string.Format("{0:D2}:{0:D2}", seconds / 60, seconds % 60);
        } else {
            return string.Format("00:{0:D2}", seconds);
        }
    }
    public Dictionary<string, object> ToInfo() {
        return new Dictionary<string, object>() {
            {"type", Source },
            {"id", ID },
            {"name", Name },
            {"album", Album },
            {"year", Year },
            {"singer", Singer.GetSingers() },
            {"path", Path.GetFullPath(FilePath) },
            {"size", new FileInfo(FilePath).Length },
            {"duration", Duration }
        };
    }
}
