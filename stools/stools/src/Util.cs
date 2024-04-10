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
        public string fileName;             //文件路径
        public bool isImage;                //是否是图片
        public string mediaType;            //媒体类型
        public DateTime createTime;         //媒体创建时间
        public decimal width;               //宽度
        public decimal height;              //高度
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
        public static MediaInfo GetMediaInfo(string fileName) {
            DirectoryList metadata;
            try {
                metadata = ImageMetadataReader.ReadMetadata(fileName);
            } catch (System.Exception) {
                //不是媒体类型
                return null;
            }
            var mediaInfo = new MediaInfo();
            mediaInfo.fileName = fileName;
            var fileType = metadata.OfType<FileTypeDirectory>().FirstOrDefault().GetDescription(3);
            mediaInfo.mediaType = fileType;
            var fileInfo = new FileInfo(fileName);
            if (fileType.Contains("image")) {
                mediaInfo.isImage = true;
                var info = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                if (info != null) {
                    var time = info.GetDescription(36867);
                    if (DateTime.TryParseExact(time, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var createTime)) {
                        mediaInfo.createTime = createTime;
                    } else {
                        mediaInfo.createTime = fileInfo.LastWriteTime;
                    }
                    mediaInfo.width = Convert.ToDecimal(info.GetObject(40962));
                    mediaInfo.height = Convert.ToDecimal(info.GetObject(40963));
                } else {
                    mediaInfo.createTime = fileInfo.LastWriteTime;
                }
            } else {
                mediaInfo.isImage = false;
                var info = metadata.OfType<QuickTimeTrackHeaderDirectory>().FirstOrDefault();
                if (info != null) {
                    var time = info.GetObject(3);
                    if (time != null) {
                        mediaInfo.createTime = (DateTime)time;
                        if (mediaInfo.createTime.Kind != DateTimeKind.Local) {
                            mediaInfo.createTime = TimeZoneInfo.ConvertTime(mediaInfo.createTime, TimeZoneInfo.Utc, TimeZoneInfo.Local);
                        }
                    } else {
                        mediaInfo.createTime = fileInfo.LastWriteTime;
                    }
                    mediaInfo.width = Convert.ToDecimal(info.GetObject(10));
                    mediaInfo.height = Convert.ToDecimal(info.GetObject(11));
                } else {
                    mediaInfo.createTime = fileInfo.LastWriteTime;
                }
            }
            //把毫秒置0
            mediaInfo.createTime = new DateTime(mediaInfo.createTime.Ticks / 10000000 * 10000000, DateTimeKind.Local);
            mediaInfo.size = fileInfo.Length;
            return mediaInfo;
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
