using Scorpio.Commons;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System;
using System.IO;
using SixLabors.ImageSharp.Processing;
using TagLib;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
namespace Scorpio.stools {
    public class MusicInfo {
        public string Name;
        public string[] Singer;
        public string Album;
        public uint? Year;
        public uint? Track;
        public string Publisher;
        public string Lyrics;
        public string[] Genres;
        public string Picture;
    }
    public class Album {
        public class Data {
            public string singer;
            public HashSet<string> performers;
            public string path;
            public List<string> files = new List<string>();
        }
        public List<Data> datas = new List<Data>();
        public Data GetData(string singer) {
            return datas.Find(x => x.singer == singer);
        }
    }
    public static class MusicUtil {
        public static Func<string, string, bool> CheckMusic;
        public static Action<MusicBase> DownloadedMusic;
        public static void SetMusicInfo(string mp3File, MusicInfo musicInfo) {
            var file = TagLib.File.Create(mp3File);
            file.Tag.Title = musicInfo.Name ?? file.Tag.Title;
            file.Tag.Performers = musicInfo.Singer ?? file.Tag.Performers;
            file.Tag.Album = musicInfo.Album ?? file.Tag.Album;
            file.Tag.Year = musicInfo.Year ?? file.Tag.Year;
            file.Tag.Track = musicInfo.Track ?? file.Tag.Track;
            file.Tag.Publisher = musicInfo.Publisher ?? file.Tag.Publisher;
            file.Tag.Lyrics = musicInfo.Lyrics ?? file.Tag.Lyrics;
            file.Tag.Genres = musicInfo.Genres ?? file.Tag.Genres;
            if (!string.IsNullOrWhiteSpace(musicInfo.Picture) && FileUtil.FileExist(musicInfo.Picture)) {
                using (Image<Rgba32> image = Image.Load<Rgba32>(musicInfo.Picture)) {
                    image.Mutate(x => x.Resize(512, 512));
                    image.Save(musicInfo.Picture);
                }
                var pictures = new List<IPicture>();
                {
                    var picture = new TagLib.Id3v2.AttachmentFrame(new Picture(musicInfo.Picture));
                    picture.Type = PictureType.FrontCover;
                    picture.Filename = "frontcover.png";
                    picture.Description = "";
                    picture.MimeType = Picture.GetMimeFromExtension(musicInfo.Picture);
                    picture.TextEncoding = StringType.Latin1;
                    pictures.Add(picture);
                }
                file.Tag.Pictures = pictures.ToArray();
            }
            file.Save();
        }
        public static void SortMusic(string source, string target, bool clear, bool move) {
            var albums = new Dictionary<string, Album>();
            void AddMusic(TagLib.Tag tag, string file) {
                var albumName = tag.Album;
                var performers = new HashSet<string>();
                Array.ForEach(tag.Performers, x => performers.UnionWith(x.Split("&")));
                var singer = string.Join("&", performers);
                var targetFile = $"{target}/{singer}/{albumName}/{singer}-{tag.Title}{Path.GetExtension(file)}";
                if (Path.GetFullPath(file) != Path.GetFullPath(targetFile)) {
                    FileUtil.MoveFile(file, targetFile, true);
                }
                if (string.IsNullOrEmpty(albumName)) return;
                if (!albums.TryGetValue(albumName, out var album)) {
                    album = albums[albumName] = new Album();
                }
                var data = album.GetData(singer);
                if (data == null) {
                    data = new Album.Data() { singer = singer, performers = performers, path = Path.GetDirectoryName(targetFile) };
                    album.datas.Add(data);
                }
                data.files.Add(targetFile);
            }
            bool MoveFile(string sourceFile, string targetFile, bool forceMove) {
                if (!System.IO.File.Exists(sourceFile))
                    return false;
                var m = move || forceMove;
                if (System.IO.File.Exists(targetFile)) {
                    if (new FileInfo(sourceFile).Length > new FileInfo(targetFile).Length) {
                        if (m)
                            FileUtil.MoveFile(sourceFile, targetFile, true);
                        else
                            FileUtil.CopyFile(sourceFile, targetFile, true);
                    } else if (m) {
                        FileUtil.DeleteFile(sourceFile);
                    }
                    return true;
                } else {
                    if (m)
                        FileUtil.MoveFile(sourceFile, targetFile, true);
                    else
                        FileUtil.CopyFile(sourceFile, targetFile, true);
                    return false;
                }
            }
            if (clear) FileUtil.DeleteFolder(target);
            if (FileUtil.PathExist(target)) {
                var files = FileUtil.GetFiles(target, "*", SearchOption.AllDirectories);
                var progress = new Progress(files.Count, "读取原数据 : ");
                for (var i = 0; i < files.Count; ++i) {
                    progress.SetProgress(i);
                    var file = files[i];
                    var musicInfo = Util.GetMusicInfo(file);
                    if (musicInfo == null || musicInfo is TagLib.Jpeg.File) continue;
                    AddMusic(musicInfo.Tag, file);
                }
            }
            var invalidCount = 0;
            var repeatCount = 0;
            var fileCount = 0;
            if (FileUtil.PathExist(source) && Path.GetFullPath(source) != Path.GetFullPath(target)) {
                var files = FileUtil.GetFiles(source, "*", SearchOption.AllDirectories);
                fileCount = files.Count;
                var progress = new Progress(fileCount, "复制文件 : ");
                for (var i = 0; i < fileCount; ++i) {
                    progress.SetProgress(i);
                    var sourceFile = files[i];
                    var musicInfo = Util.GetMusicInfo(sourceFile);
                    if (musicInfo == null || musicInfo is TagLib.Jpeg.File) {
                        invalidCount++;
                        continue;
                    }
                    var albumName = musicInfo.Tag.Album;
                    var performers = new HashSet<string>();
                    Array.ForEach(musicInfo.Tag.Performers, x => performers.UnionWith(x.Split("&")));
                    var singer = string.Join("&", performers);
                    var targetFile = $"{target}/{singer}/{albumName}/{singer}-{musicInfo.Tag.Title}{Path.GetExtension(sourceFile)}";
                    if (MoveFile(sourceFile, targetFile, false))
                        repeatCount++;
                    AddMusic(musicInfo.Tag, targetFile);
                }
            }
            void SortAlbum(Album album) {
                while (album.datas.Count > 1) {
                    album.datas.Sort((a, b) => b.files.Count.CompareTo(a.files.Count));
                    var first = album.datas[0];
                    album.datas.RemoveAt(0);
                    for (var i = 0; i < album.datas.Count; ++i) {
                        var data = album.datas[i];
                        if (first.performers.Overlaps(data.performers)) {
                            foreach (var file in data.files) {
                                var targetFile = $"{first.path}/{Path.GetFileName(file)}";
                                logger.info($"移动文件 {file} -> {targetFile}");
                                if (MoveFile(file, targetFile, true)) {
                                    repeatCount++;
                                }
                            }
                        }
                    }
                }
            }
            {
                var progress = new Progress(albums.Count, "整理专辑 : ");
                var index = 0;
                foreach (var pair in albums) {
                    progress.SetProgress(index++);
                    SortAlbum(pair.Value);
                }
            }
            logger.info("清理空目录");
            FileUtil.DeleteEmptyFolder(source, true);
            FileUtil.DeleteEmptyFolder(target, true);
            logger.info($"整理完成,所有文件:{fileCount},无效文件:{invalidCount},重复文件:{repeatCount}");
        }
        public static async Task<T> Get<T>(this string url) {
            return JsonConvert.DeserializeObject<T>(await HttpUtil.Get(url));
        }
        public static async Task DownloadAlbumUrls(IEnumerable<string> urls, string output, MusicPath musicPath) {
            foreach (var url in urls) {
                try {
                    await DownloadAlbumUrl(url, output, musicPath);
                } catch (System.Exception e) {
                    logger.error($"下载专辑失败[{url}] : {e}");
                }
                await Task.Delay(3000);
            }
        }
        private static async Task DownloadAlbumUrl(string url, string output, MusicPath musicPath) {
            if (string.IsNullOrWhiteSpace(url)) { return; }
            if (FileUtil.FileExist(url)) { url = Path.GetFullPath(url); }
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFile) {
                var lines = System.IO.File.ReadAllLines(uri.LocalPath);
                foreach (var line in lines) {
                    if (line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("!")) { continue; }
                    await DownloadAlbumUrl(line, output, musicPath);
                }
                return;
            }
            string type;
            string id;
            if (uri.Host.Contains("kuwo")) {
                type = MusicFactory.Kuwo;
                id = url.Substring(url.LastIndexOf("/") + 1);
                if (id.IndexOf("?") >= 0) {
                    id = id.Substring(0, id.IndexOf("?"));
                }
            } else if (uri.Host.Contains("163")) {
                type = MusicFactory.Cloud;
                id = Regex.Match(url, "id=\\w+(&|$)").ToString().Substring(3);
                if (id.EndsWith("&")) {
                    id = id.Substring(0, id.Length - 1);
                }
            } else if (uri.Host.Contains("kugou")) {
                type = MusicFactory.Kugou;
                if (url.EndsWith("/")) url = url.Substring(0, url.Length - 1);
                id = url.Substring(url.LastIndexOf("/") + 1);
            } else {
                throw new System.Exception($"不支持的源数据:{url}");
            }
            await DownloadAlbum(type, id, output, musicPath);
        }
        private static async Task DownloadAlbum(string type, string id, string output, MusicPath musicPath) {
            var albumInfo = await MusicFactory.Create(type).ParseAlbum(id);
            foreach (var musicid in albumInfo.musicList) {
                await DownloadMusic(type, musicid, output, musicPath);
                await Task.Delay(3000);
            }
        }
        public static async Task DownloadMusicUrls(IEnumerable<string> urls, string output, MusicPath musicPath) {
            foreach (var url in urls) {
                try {
                    await DownloadMusicUrl(url, output, musicPath);
                } catch (System.Exception e) {
                    logger.error($"下载链接失败[{url}] : {e}");
                }
                await Task.Delay(3000);
            }
        }
        private static async Task DownloadMusicUrl(string url, string output, MusicPath musicPath) {
            if (string.IsNullOrWhiteSpace(url)) { return; }
            if (FileUtil.FileExist(url)) { url = Path.GetFullPath(url); }
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFile) {
                var lines = System.IO.File.ReadAllLines(uri.LocalPath);
                foreach (var line in lines) {
                    if (line.StartsWith("#") || line.StartsWith(";")) { continue; }
                    await DownloadMusicUrl(line, output, musicPath);
                }
                return;
            }
            string type;
            string id;
            if (uri.Host.Contains("kuwo")) {
                type = MusicFactory.Kuwo;
                id = url.Substring(url.LastIndexOf("/") + 1);
                if (id.IndexOf("?") >= 0) {
                    id = id.Substring(0, id.IndexOf("?"));
                }
            } else if (uri.Host.Contains("qq.com")) {
                type = MusicFactory.QQ;
                id = url.Substring(url.LastIndexOf("/") + 1);
                if (id.IndexOf("?") >= 0) {
                    id = id.Substring(0, id.IndexOf("?"));
                }
            } else if (uri.Host.Contains("kugou")) {
                type = MusicFactory.Kugou;
                var result = await HttpUtil.Get(url);
                var info = JsonConvert.DeserializeObject<MusicKugou.MusicUrlInfo>(Regex.Match(result, "(?<=dataFromSmarty = \\[).*?(?=],)").ToString());
                id = info.hash;
            } else if (uri.Host.Contains("163")) {
                type = MusicFactory.Cloud;
                id = Regex.Match(url, "id=\\w+(&|$)").ToString().Substring(3);
                if (id.EndsWith("&")) {
                    id = id.Substring(0, id.Length - 1);
                }
            } else {
                throw new System.Exception($"不支持的源数据:{url}");
            }
            await DownloadMusic(type, id, output, musicPath);
        }
        private static async Task DownloadMusic(string type, string id, string output, MusicPath musicPath) {
            if (CheckMusic?.Invoke(type, id) ?? true) {
                try {
                    var music = MusicFactory.Create(type);
                    if (await music.Download(id, output, musicPath)) {
                        DownloadedMusic?.Invoke(music);
                    }
                } catch (System.Exception e) {
                    logger.error(e.ToString());
                }
            }
        }
    }
}
