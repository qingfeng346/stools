using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Linq;
using Scorpio.Commons;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.FileType;
using MetadataExtractor.Formats.Exif;
using System.Globalization;
using MetadataExtractor.Formats.QuickTime;
using System.Collections;
using Newtonsoft.Json;
using System.IO.Compression;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor.Formats.Png;
using MetadataExtractor.Formats.Bmp;
using MetadataExtractor.Formats.Gif;
using MetadataExtractor.Formats.WebP;
using MetadataExtractor.Formats.Avi;

#if NET35
using DirectoryList = System.Collections.Generic.IList<MetadataExtractor.Directory>;
#else
using DirectoryList = System.Collections.Generic.IReadOnlyList<MetadataExtractor.Directory>;
#endif

namespace Scorpio.stools {
    public class TSData {
        public int index;
        public List<string> urls = new List<string>();
        public string Name => string.Format("{0:00000}.ts", index);
        public string Urls => string.Join(";", urls);
    }
    public class MusicCache {
        public Dictionary<string, HashSet<string>> music = new ();
    }
    public class MediaInfo {
        private string _fileName;
        //文件路径
        public string fileName {
            get => _fileName;
            set {
                _fileName = value;
                _md5 = null;
            }
        }
        public bool isImage;                //是否是图片
        public bool isTime;                 //是否有拍摄时间
        public string mediaType;            //媒体类型
        public DateTime? createTime;        //媒体创建时间
        public long? width;                 //宽度
        public long? height;                //高度
        public long size;                   //文件大小
        private string _md5 = null;
        public string md5 => _md5 ??= FileUtil.GetMD5FromFileStream(fileName);
        public override int GetHashCode() {
            var comparer = (IEqualityComparer)EqualityComparer<object>.Default;
            return CombineHashCodes(comparer.GetHashCode(mediaType),
                                    comparer.GetHashCode(size),
                                    comparer.GetHashCode(width),
                                    comparer.GetHashCode(height));
        }
        internal static int CombineHashCodes(int h1, int h2) {
            return ((h1 << 5) + h1) ^ h2;
        }
        internal static int CombineHashCodes(int h1, int h2, int h3) {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }
        internal static int CombineHashCodes(int h1, int h2, int h3, int h4) {
            return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
        }
        public override bool Equals(object obj) {
            var other = obj as MediaInfo;
            if (other == null) { return false; }
            return mediaType == other.mediaType &&
                   size == other.size &&
                   width == other.width &&
                   height == other.height &&
                   md5 == other.md5;
        }
        public override string ToString() {
            return $"{mediaType}-{width}x{height}-{size}-{createTime}";
        }
    }
    public static class Util {
        public static Func<string, string, bool> CheckMusic;
        public static Action<MusicBase> DownloadedMusic;
        public static void ExecuteAndroidpublisher(string authFile, string packageName, Action<AndroidPublisherService, string> action, Action<TrackRelease> trackAction) {
            var service = new AndroidPublisherService(new BaseClientService.Initializer() {
                HttpClientInitializer = GoogleCredential.FromFile(authFile).CreateScoped("https://www.googleapis.com/auth/androidpublisher"),
                ApplicationName = packageName,
            });
            var editId = service.Edits.Insert(new AppEdit() { ExpiryTimeSeconds = "3600" }, packageName).Execute().Id;
            action(service, editId);
            var trackInfo = service.Edits.Tracks.Get(packageName, editId, "internal").Execute();
            var release = trackInfo.Releases.SingleOrDefault(release => release?.Status == "draft", null);
            if (release == null) {
                release = new TrackRelease() { Status = "draft" };
                trackInfo.Releases.Add(release);
            }
            trackAction(release);
            service.Edits.Tracks.Update(trackInfo, packageName, editId, "internal").Execute();
            var commit = service.Edits.Commit(packageName, editId);
            commit.ChangesNotSentForReview = false;
            commit.Execute();
        }
        public static string GetMobileprovisionInfo(string file) {
            return ScorpioUtil.Execute("security", null, new string[] {
                "cms",
                "-D",
                "-i",
                file
            }).output;
        }
        public static int ExecuteFFmpeg(params string[] args) {
            if (ScorpioUtil.IsMacOS()) {
                return ScorpioUtil.Execute("ffmpeg", null, args).exitCode;
            } else if (ScorpioUtil.IsWindows()) {
                return ScorpioUtil.Execute("ffmpeg.exe", null, args).exitCode;
            } else if (ScorpioUtil.IsLinux()) {
                return ScorpioUtil.Execute("ffmpeg", null, args).exitCode;
            }
            return -1;
        }
        static DirectoryList ReadMetadata(string fileName) {
            try {
                return ImageMetadataReader.ReadMetadata(fileName);
            } catch (System.Exception) { }
            try {
                if (fileName.EndsWith(".livp")) {
                    using (var fileStream = File.OpenRead(fileName)) {
                        using (var zipArchive =  new ZipArchive(fileStream)) {
                            foreach (var entry in zipArchive.Entries) {
                                var tempFile = Path.GetTempFileName();
                                using (var tempStream = File.OpenWrite(tempFile)) {
                                    using (var entryStream = entry.Open()) {
                                        entryStream.CopyTo(tempStream);
                                    }
                                }
                                try {
                                    var list = ImageMetadataReader.ReadMetadata(tempFile);
                                    var fileType = list.OfType<FileTypeDirectory>().FirstOrDefault().GetDescription(FileTypeDirectory.TagDetectedFileMimeType);
                                    if (fileType.Contains("image"))
                                        return list;
                                } catch (System.Exception e) {
                                    logger.error("读取Entry信息失败 : " + entry.Name + " " + e.ToString());
                                } finally {
                                    FileUtil.DeleteFile(tempFile);
                                }
                            }
                        }
                    }
                }
            } catch (System.Exception) { }
            return null;
        }
        static DateTime? GetDateTime(object? value, string format = "yyyy:MM:dd HH:mm:ss") {
            if (value == null) {
                return null;
            } else if (value is StringValue || value is string) {
                if (string.IsNullOrWhiteSpace(value.ToString())) return null;
                if (DateTime.TryParseExact(value.ToString(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time)) {
                    return time;
                }
                throw new System.Exception($"未知的时间格式 : {value}");
            } else if (value is DateTime) {
                return (DateTime)value;
            } else {
                throw new System.Exception($"未知的时间类型 : {value.GetType()} - {value}");
            }
        }
        public static MediaInfo GetMediaInfo(string fileName) {
            var metadata = ReadMetadata(fileName);
            if (metadata == null) return null;
            try {
                var mediaInfo = new MediaInfo();
                mediaInfo.fileName = fileName;
                var fileType = metadata.OfType<FileTypeDirectory>().FirstOrDefault().GetDescription(FileTypeDirectory.TagDetectedFileMimeType);
                mediaInfo.mediaType = fileType;
                var fileInfo = new FileInfo(fileName);
                if (fileType.Contains("image")) {
                    mediaInfo.isImage = true;
                    foreach (var info in metadata) {
                        if (mediaInfo.createTime == null) {
                            DateTime? time = null;
                            if (info is ExifIfd0Directory || info is ExifSubIfdDirectory) {
                                var time1 = GetDateTime(info.GetObject(ExifDirectoryBase.TagDateTimeDigitized));
                                var time2 = GetDateTime(info.GetObject(ExifDirectoryBase.TagDateTimeOriginal));
                                var time3 = GetDateTime(info.GetObject(ExifDirectoryBase.TagDateTime));
                                time = time1;
                                if (time2 != null && (time == null || time2 > time)) {
                                    time = time2;
                                }
                                if (time3 != null && (time == null || time3 > time)) {
                                    time = time3;
                                }
                            }
                            if (time != null) {
                                mediaInfo.isTime = true;
                                mediaInfo.createTime = time;
                            }
                        }
                        if (mediaInfo.width == null) {
                            object? width = null, height = null;
                            if (info is ExifIfd0Directory) {
                                width = info.GetObject(ExifDirectoryBase.TagImageWidth);
                                height = info.GetObject(ExifDirectoryBase.TagImageHeight);
                            } else if (info is ExifSubIfdDirectory) {
                                width = info.GetObject(ExifDirectoryBase.TagExifImageWidth);
                                height = info.GetObject(ExifDirectoryBase.TagExifImageHeight);
                            } else if (info is JpegDirectory) {
                                width = info.GetObject(JpegDirectory.TagImageWidth);
                                height = info.GetObject(JpegDirectory.TagImageHeight);
                            } else if (info is PngDirectory) {
                                width = info.GetObject(PngDirectory.TagImageWidth);
                                height = info.GetObject(PngDirectory.TagImageHeight);
                            } else if (info is BmpHeaderDirectory) {
                                width = info.GetObject(BmpHeaderDirectory.TagImageWidth);
                                height = info.GetObject(BmpHeaderDirectory.TagImageHeight);
                            } else if (info is GifImageDirectory) {
                                width = info.GetObject(GifImageDirectory.TagWidth);
                                height = info.GetObject(GifImageDirectory.TagHeight);
                            } else if (info is GifHeaderDirectory) {
                                width = info.GetObject(GifHeaderDirectory.TagImageWidth);
                                height = info.GetObject(GifHeaderDirectory.TagImageHeight);
                            } else if (info is WebPDirectory) {
                                width = info.GetObject(WebPDirectory.TagImageWidth);
                                height = info.GetObject(WebPDirectory.TagImageHeight);
                            }
                            if (width != null && height != null) {
                                mediaInfo.width = Convert.ToInt64(width);
                                mediaInfo.height = Convert.ToInt64(height);
                            }
                        }
                    }
                    if (mediaInfo.createTime == null) {
                        mediaInfo.isTime = false;
                        var lastWriteTime = fileInfo.LastWriteTime;
                        logger.info($"读取图片拍摄时间失败[{fileName}],自动设置为最后修改时间:{lastWriteTime}");
                        mediaInfo.createTime = lastWriteTime;
                    }
                    if (mediaInfo.width == null || mediaInfo.height == null) {
                        throw new System.Exception($"读取图片宽高失败[{fileName}], width:{mediaInfo.width} height:{mediaInfo.height}");
                    }
                } else {
                    mediaInfo.isImage = false;
                    foreach (var info in metadata) {
                        if (mediaInfo.createTime == null) {
                            object? time = null;
                            if (info is QuickTimeTrackHeaderDirectory) {
                                time = info.GetObject(QuickTimeTrackHeaderDirectory.TagCreated);
                            } else if (info is QuickTimeMovieHeaderDirectory) {
                                time = info.GetObject(QuickTimeMovieHeaderDirectory.TagCreated);
                            } else if (info is AviDirectory) {
                                time = info.GetObject(AviDirectory.TagDateTimeOriginal);
                            }
                            if (time != null) {
                                mediaInfo.isTime = true;
                                mediaInfo.createTime = GetDateTime(time, "ddd MMM dd HH:mm:ss yyyy");
                            }
                        }
                        if (mediaInfo.width == null) {
                            object? width = null, height = null;
                            if (info is QuickTimeTrackHeaderDirectory) {
                                width = info.GetObject(QuickTimeTrackHeaderDirectory.TagWidth);
                                height = info.GetObject(QuickTimeTrackHeaderDirectory.TagHeight);
                            } else if (info is AviDirectory) {
                                width = info.GetObject(AviDirectory.TagWidth);
                                height = info.GetObject(AviDirectory.TagHeight);
                            }
                            if (width != null && height != null && Convert.ToInt64(width) != 0 && Convert.ToInt64(height) > 0) {
                                mediaInfo.width = Convert.ToInt64(width);
                                mediaInfo.height = Convert.ToInt64(height);
                            }
                        }
                    }
                    if (mediaInfo.createTime == null) {
                        mediaInfo.isTime = false;
                        var lastWriteTime = fileInfo.LastWriteTime;
                        logger.info($"读取视频拍摄时间失败[{fileName}],自动设置为最后修改时间:{lastWriteTime}");
                        mediaInfo.createTime = lastWriteTime;
                    }
                    if (mediaInfo.width == null || mediaInfo.height == null) {
                        throw new System.Exception($"读取视频宽高失败[{fileName}], width:{mediaInfo.width} height:{mediaInfo.height}");
                    }
                }
                //把毫秒置0
                mediaInfo.createTime = new DateTime(mediaInfo.createTime.Value.Ticks / 10000000 * 10000000, DateTimeKind.Local);
                mediaInfo.size = fileInfo.Length;
                return mediaInfo;
            } catch (System.Exception e) {
                throw new System.Exception($"文件:{fileName} 解析失败:{e}");
            }
            
        }
        public static TagLib.File GetMusicInfo(string fileName) {
            try {
                return TagLib.File.Create(fileName);
            } catch(System.Exception) {
                return null;
            }
        }
        public static string GetFilenameByUrl(string url) {
            url = url.Substring(url.LastIndexOf("/") + 1);
            var index = url.IndexOf("?");
            if (index >= 0) {
                return url.Substring(0, index);
            }
            return url;
        }
        public static async Task DownloadAlbumUrls(IEnumerable<string> urls, string output, MusicPath musicPath) {
            foreach (var url in urls) {
                await DownloadAlbumUrl(url, output, musicPath);
            }
        }
        public static async Task DownloadAlbumUrl(string url, string output, MusicPath musicPath) {
            if (string.IsNullOrWhiteSpace(url)) { return; }
            if (FileUtil.FileExist(url)) { url = Path.GetFullPath(url); }
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFile) {
                var lines = File.ReadAllLines(uri.LocalPath);
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
        public static async Task DownloadAlbum(string type, string id, string output, MusicPath musicPath) {
            var albumInfo = await MusicFactory.Create(type).ParseAlbum(id);
            foreach (var musicid in albumInfo.musicList) {
                await DownloadMusic(type, musicid, output, musicPath);
            }
        }
        public static async Task DownloadMusicUrls(IEnumerable<string> urls, string output, MusicPath musicPath) {
            foreach (var url in urls) {
                await DownloadMusicUrl(url, output, musicPath);
            }
        }
        public static async Task DownloadMusicUrl(string url, string output, MusicPath musicPath) {
            if (string.IsNullOrWhiteSpace(url)) { return; }
            if (FileUtil.FileExist(url)) { url = Path.GetFullPath(url); }
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFile) {
                var lines = File.ReadAllLines(uri.LocalPath);
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
        public static async Task DownloadMusic(string type, string id, string output, MusicPath musicPath) {
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
