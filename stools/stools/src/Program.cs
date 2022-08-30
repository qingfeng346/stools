using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Scorpio.Commons;
namespace Scorpio.stools {
    class Program {
        private const string ParameterAuth = "--auth|-auth";
        private const string ParameterPackageName = "--packageName|-packageName";
        private const string ParameterVersion = "--version|-version";
        private const string ParameterTrackName = "--trackName|-trackName";
        private const string ParameterApk = "--apk|-apk";
        private const string ParameterObb = "--obb|-obb";
        private const string ParameterReleasenote = "--releasenote|-releasenote";
        private const string ParameterUsername = "--username|-username|-u";
        private const string ParameterPassword = "--password|-password|-p";
        private const string ParameterAppleid = "--appleid|-appleid|--apple_id|-apple_id|-id";
        private const string ParameterOutput = "--output|-output|-o";
        private const string ParameterFile = "--file|-file|-f";
        private const string ParameterType = "--type|-type|-t";
        private const string ParameterID = "--id|-id";
        private const string ParameterUrl = "--url|-url";
        private const string ParameterNamePath = "--namepath|-namepath";
        private const string ParameterIpa = "--ipa|-ipa";
        private const string ParameterProvision = "--provision|-provision|-p";
        private const string ParameterDeveloper = "--developer|-developer|-d";
        private const string ParameterQueue = "--queue|-queue|-q";
        public class TSData {
            public int index;
            public List<string> urls = new List<string>();
            public string Name => string.Format("{0:00000}.ts", index);
            public string Urls => string.Join(";", urls);
        }
        static void Main (string[] args) {
            Encoding.RegisterProvider (CodePagesEncodingProvider.Instance);
            var perform = new Perform ();
            perform.AddExecute ("androidpublisher", "更新Google Play信息", Androidpublisher);
            perform.AddExecute ("lookupMetadata", "获取AppStore Metadata文件", LookupMetadata);
            perform.AddExecute ("uploadMetadata", "更新AppStore Metadata文件", UploadMetadata);
            perform.AddExecute ("lookupMobileprovision", "查看mobileprovision文件", LookupMobileprovision);
            perform.AddExecute ("resign", "ipa文件重签名", Resign);
            perform.AddExecute ("wget", "下载文件", Wget);
            perform.AddExecute ("downloadAlbum", "下载音乐专辑", DownloadAlbum);
            perform.AddExecute ("downloadMusic", "下载音乐", DownloadMusic);
            perform.AddExecute ("downloadM3u8", "下载M3U8文件", DownloadM3u8);
            try {
                perform.Start (args);
                Environment.Exit(0);
            } catch (System.Exception e) {
                Console.Error.WriteLine (e);
                Environment.Exit(1);
            }
        }
        static void ExecuteAndroidpublisher (string authFile, string packageName, Action<AndroidPublisherService, string> action, Action<TrackRelease> trackAction) {
            var service = new AndroidPublisherService (new BaseClientService.Initializer () {
                HttpClientInitializer = GoogleCredential.FromFile (authFile).CreateScoped ("https://www.googleapis.com/auth/androidpublisher"),
                    ApplicationName = packageName,
            });
            var editId = service.Edits.Insert (new AppEdit () { ExpiryTimeSeconds = "3600" }, packageName).Execute ().Id;
            action (service, editId);
            var trackInfo = service.Edits.Tracks.Get (packageName, editId, "internal").Execute ();
            var release = trackInfo.Releases.SingleOrDefault (release => release?.Status == "draft", null);
            if (release == null) {
                release = new TrackRelease () { Status = "draft" };
                trackInfo.Releases.Add (release);
            }
            trackAction (release);
            service.Edits.Tracks.Update (trackInfo, packageName, editId, "internal").Execute ();
            var commit = service.Edits.Commit (packageName, editId);
            commit.ChangesNotSentForReview = false;
            commit.Execute ();
        }
        static string GetMobileprovisionInfo (string file) {
            var info = "";
            ScorpioUtil.StartProcess ("security", null, new string[] {
                "cms",
                "-D",
                "-i",
                file
            }, null, (process) => {
                info = process.StandardOutput.ReadToEnd ();
            });
            return info;
        }
        static void Androidpublisher ([ParamterInfo("服务账号json文件", ParameterAuth)]string auth,
                                      [ParamterInfo("PackageName", ParameterPackageName)]string packageName,
                                      [ParamterInfo("VersionCode", ParameterVersion, false)]int version,
                                      [ParamterInfo("轨道名字", ParameterTrackName, false)] string trackName,
                                      [ParamterInfo("Apk文件", ParameterApk, false)] string apk,
                                      [ParamterInfo("Obb文件", ParameterObb, false)] string obb,
                                      [ParamterInfo("ReleaseNote文件(xml文件)", ParameterReleasenote, false)] string releaseNote) {
            ExecuteAndroidpublisher (auth, packageName, (service, editId) => {
                    if (!string.IsNullOrWhiteSpace (apk)) {
                        var apkSize = new FileInfo (apk).Length;
                        var apkUpload = service.Edits.Apks.Upload (packageName, editId, File.OpenRead (apk), "application/vnd.android.package-archive");
                        apkUpload.ProgressChanged += (progress) => {
                            Console.WriteLine ($"上传{apk}进度:{progress.BytesSent.GetMemory()}/{apkSize.GetMemory()}");
                        };
                        Console.WriteLine ($"开始上传apk文件:{apk}");
                        var apkResult = apkUpload.Upload ();
                        if (apkResult.Exception != null) {
                            throw apkResult.Exception;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace (obb)) {
                        var obbSize = new FileInfo (obb).Length;
                        var obbUpload = service.Edits.Expansionfiles.Upload (packageName, editId, version, EditsResource.ExpansionfilesResource.UploadMediaUpload.ExpansionFileTypeEnum.Main, File.OpenRead (obb), "application/octet-stream");
                        obbUpload.ProgressChanged += (progress) => {
                            Console.WriteLine ($"上传{obb}进度:{progress.BytesSent.GetMemory()}/{obbSize.GetMemory()}");
                        };
                        Console.WriteLine ($"开始上传obb文件:{obb}");
                        var obbResult = obbUpload.Upload ();
                        if (obbResult.Exception != null) {
                            throw obbResult.Exception;
                        }
                    }
                },
                (release) => {
                    if (!string.IsNullOrWhiteSpace (trackName)) { release.Name = trackName; }
                    if (!string.IsNullOrWhiteSpace (releaseNote)) {
                        release.ReleaseNotes = new List<LocalizedText> ();
                        var xmlDoc = new XmlDocument ();
                        xmlDoc.Load (releaseNote);
                        foreach (XmlNode language in xmlDoc.DocumentElement) {
                            release.ReleaseNotes.Add (new LocalizedText () {
                                Language = language.Name,
                                    Text = language.InnerText.Trim ()
                            });
                        }
                    }
                    release.VersionCodes = new List<long?> () { version };
                });
        }
        static void ExecuteTMSTransporter (string username, string password, IEnumerable<string> args) {
            if (ScorpioUtil.IsMacOS ()) {
                var argList = new List<string> () {
                    "iTMSTransporter",
                    "-u",
                    username,
                    "-p",
                    password
                };
                argList.AddRange (args);
                ScorpioUtil.StartProcess ("xcrun", null, argList);
            } else if (ScorpioUtil.IsWindows ()) {
                var argList = new List<string> () {
                    "-u",
                    username,
                    "-p",
                    password
                };
                argList.AddRange (args);
                ScorpioUtil.StartCwd ("iTMSTransporter.cmd", null, argList);
            } else if (ScorpioUtil.IsLinux ()) {
                var argList = new List<string> () {
                    "-u",
                    username,
                    "-p",
                    password
                };
                argList.AddRange (args);
                ScorpioUtil.StartProcess ("iTMSTransporter", null, argList);
            }
        }
        static int ExecuteFFmpeg(params string[] args) {
            if (ScorpioUtil.IsMacOS()) {
                return ScorpioUtil.StartProcess("ffmpeg", null, args);
            } else if (ScorpioUtil.IsWindows()) {
                return ScorpioUtil.StartProcess("ffmpeg.exe", null, args, (process) => {
                    process.StartInfo.CreateNoWindow = false;
                });
            } else if (ScorpioUtil.IsLinux()) {
                return ScorpioUtil.StartProcess("ffmpeg", null, args);
            }
            return -1;
        }
        static void LookupMetadata ([ParamterInfo("账号", ParameterUsername)] string username,
                                    [ParamterInfo("密码", ParameterPassword)] string password,
                                    [ParamterInfo("AppleId", ParameterAppleid)] string appleid,
                                    [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
            ExecuteTMSTransporter (username, password, new [] { "-m", "lookupMetadata", "-apple_id", appleid, "-destination", output });
        }
        static void UploadMetadata ([ParamterInfo("账号", ParameterUsername)] string username,
                                    [ParamterInfo("密码", ParameterPassword)] string password,
                                    [ParamterInfo("文件", ParameterFile)] string file) {
            ExecuteTMSTransporter (username, password, new [] { "-m", "upload", "-f", file });
        }
        static void LookupMobileprovision ([ParamterInfo("文件", ParameterFile)] string file) {
            Console.WriteLine (GetMobileprovisionInfo (file));
        }
        static void Resign ([ParamterInfo("ipa文件", ParameterIpa)] string ipa,
                            [ParamterInfo("描述文件", ParameterProvision)] string provision,
                            [ParamterInfo("开发者名称", ParameterDeveloper)] string developer,
                            [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
            var resign = "ios_resign_with_ipa.sh";
            if (string.IsNullOrEmpty (developer)) {
                var mobileprovisionInfo = GetMobileprovisionInfo (provision);
                var xmlDoc = new XmlDocument ();
                xmlDoc.LoadXml (mobileprovisionInfo);
                var dict = xmlDoc.DocumentElement["dict"];
                foreach (XmlNode node in dict) {
                    if (node.Name == "key" && node.InnerText == "TeamName") {
                        developer = $"iPhone Distribution: {node.NextSibling.InnerText}";
                        break;
                    }
                }
            }
            File.Copy ($"{ScorpioUtil.BaseDirectory}/{resign}", resign, true);
            ScorpioUtil.StartProcess ("sh", null, new string[] {
                resign,
                ipa,
                developer,
                provision,
                output
            });
            FileUtil.DeleteFile (resign);
        }
        static string GetFilenameByUrl(string url) {
            url = url.Substring(url.LastIndexOf("/") + 1);
            var index = url.IndexOf("?");
            if (index >= 0) {
                return url.Substring(0, index);
            }
            return url;
        }
        static void Wget (CommandLine commandLine,
                         [ParamterInfo("下载URL", ParameterUrl, false)] string[] url,
                         [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
            var urls = new List<string> ();
            urls.AddRange (commandLine.Args);
            if (url != null) urls.AddRange(url);
            var tasks = new List<Task> ();
            for (var i = 0; i < urls.Count; i++) {
                var uri = urls[i];
                tasks.Add (Task.Run (async () => {
                    logger.info ($"开始下载 : {uri}");
                    await HttpUtil.Download (uri, output + GetFilenameByUrl(uri));
                    logger.info ($"下载完成 : {uri}");
                }));
            }
            Task.WaitAll (tasks.ToArray ());
        }
        static void DownloadAlbum(CommandLine commandLine,
                                  [ParamterInfo("专辑URL(酷我,网易云)", ParameterUrl, false)] string[] url,
                                  [ParamterInfo("是否创建歌手专辑目录", ParameterNamePath, false)] bool namepath,
                                  [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
            var tasks = new List<Task>();
            var urls = new List<string>();
            urls.AddRange(commandLine.Args);
            if (url != null) urls.AddRange(url);
            var ids = commandLine.GetValues(ParameterID);
            Task.WaitAll(Task.Run(async () => {
                if (ids.Length > 0) {
                    var music = MusicFactory.Create(commandLine.GetValueDefault(ParameterType, ""));
                    foreach (var id in ids) {
                        var albumInfo = await music.ParseAlbum(id);
                        var outputPath = namepath ? Path.Combine(output, albumInfo.name) : output;
                        foreach (var musicid in albumInfo.musicList) {
                            await music.Download(musicid, outputPath);
                        }
                    }
                }
                foreach (var url in urls) {
                    await DownloadAlbumUrl(url, output, namepath);
                }
            }));
        }
        static async Task DownloadAlbumUrl(string url, string output, bool createPath) {
            if (string.IsNullOrWhiteSpace(url)) { return; }
            if (FileUtil.FileExist(url)) { url = Path.GetFullPath(url); }
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFile) {
                var lines = File.ReadAllLines(uri.LocalPath);
                foreach (var line in lines) {
                    if (line.StartsWith("#") || line.StartsWith(";") || line.StartsWith("!")) { continue; }
                    await DownloadAlbumUrl(line, output, createPath);
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
            } else {
                throw new System.Exception($"不支持的源数据:{url}");
            }
            var music = MusicFactory.Create(type);
            var albumInfo = await music.ParseAlbum(id);
            var outputPath = createPath ? Path.Combine(output, albumInfo.artist, albumInfo.name) : output;
            foreach (var musicid in albumInfo.musicList) {
                await music.Download(musicid, outputPath);
            }
        }
        static void DownloadMusic (CommandLine commandLine,
                                  [ParamterInfo("音乐URL(酷我,网易云)", ParameterUrl, false)] string[] url,
                                  [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
            var tasks = new List<Task> ();
            var urls = new List<string> ();
            urls.AddRange (commandLine.Args);
            if (url != null) urls.AddRange(url);
            var ids = commandLine.GetValues (ParameterID);
            Task.WaitAll(Task.Run(async () => {
                if (ids.Length > 0) {
                    var music = MusicFactory.Create(commandLine.GetValueDefault(ParameterType, ""));
                    foreach (var id in ids) {
                        await music.Download(id, output);
                    }
                }
                foreach (var url in urls) {
                    await DownloadMusicUrl(url, output);
                }
            }));
        }
        static async Task DownloadMusicUrl(string url, string output) {
            if (string.IsNullOrWhiteSpace(url)) { return; }
            if (FileUtil.FileExist(url)) { url = Path.GetFullPath(url); }
            var uri = new Uri(url);
            if (uri.Scheme == Uri.UriSchemeFile) {
                var lines = File.ReadAllLines(uri.LocalPath);
                foreach (var line in lines) {
                    if (line.StartsWith("#") || line.StartsWith(";")) { continue; }
                    await DownloadMusicUrl(line, output);
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
                id = Regex.Match(url, "hash=\\w+(&|$)").ToString().Substring(5);
                if (id.EndsWith("&")) {
                    id = id.Substring(0, id.Length - 1);
                }
            } else if (uri.Host.Contains("163")) {
                type = MusicFactory.Cloud;
                id = Regex.Match(url, "id=\\w+(&|$)").ToString().Substring(3);
                if (id.EndsWith("&")) {
                    id = id.Substring(0, id.Length - 1);
                }
            } else {
                throw new System.Exception($"不支持的源数据:{url}");
            }
            var music = MusicFactory.Create(type);
            await music.Download(id, output);
        }
        static void DownloadM3u8 (CommandLine commandLine,
                                  [ParamterInfo("M3U8链接", ParameterUrl, false)] string url,
                                  [ParamterInfo("下载队列数", ParameterQueue, "8", false)] int queue,
                                  [ParamterInfo("输出文件", ParameterOutput, false)] string output) {
            if (string.IsNullOrEmpty(url)) {
                url = commandLine.Args.Count > 0 ? commandLine.Args[0] : null;
            }
            if (string.IsNullOrEmpty(url)) {
                throw new System.Exception("m3u8 url 不能为空");
            }
            if (string.IsNullOrEmpty(output)) {
                output = Path.GetFullPath(FileUtil.GetMD5FromString(url) + ".mp4");
            }
            queue = Math.Max(1, queue);
            string m3u8Content = "";
            Task.WaitAll(Task.Run(async () => { m3u8Content = await HttpUtil.Get(url); }));
            var baseUrl = url.Substring(0, url.IndexOf("/", url.StartsWith("http") ? 7 : 8));
            var parentUrl = url.Substring(0, url.LastIndexOf("/") + 1);
            var lines = m3u8Content.Split("\n");
            var index = 1;
            var tasks = new List<Task<string>>();
            var tsBase = $"{output}.ts";
            FileUtil.CreateDirectory(tsBase);
            var tsCount = 0;
            var tsList = new Queue<TSData>();
            for (var i = 0; i < lines.Length; i++) {
                var line = lines[i];
                if (line.StartsWith("#EXTINF")) {
                    var tsUrl = lines[++i];
                    var tsData = new TSData() { index = index++ };
                    if (tsUrl.StartsWith("http")) {
                        tsData.urls.Add(tsUrl);
                    } else {
                        tsData.urls.Add(parentUrl + tsUrl);
                        tsData.urls.Add(baseUrl + tsUrl);
                    } 
                    tsList.Enqueue(tsData);
                    lines[i] = $"file {tsData.Name}";
                }
            }
            Console.WriteLine($"start download -> {output}");
            var sync = new object();
            var tsTotal = tsList.Count;
            var downloaded = 0L;
            //开启16个线程同时下载
            for (var i = 0; i < queue; i++) {
                var task = Task.Run(async () => {
                    try {
                    Start:
                        TSData data = null;
                        lock (sync) {
                            if (tsList.Count > 0) {
                                data = tsList.Dequeue();
                            }
                        }
                        if (data == null) { 
                            return "";
                        }
                        var isSuccess = false;
                        foreach (var url in data.urls) {
                            var result = await HttpUtil.Download(url, $"{tsBase}/{data.Name}", false, true);
                            if (result.IsSuccessStatusCode) {
                                isSuccess = true;
                                downloaded += result.Length;
                                if (result.Skip) {
                                    logger.info($"[跳过] 下载进度:{++tsCount}/{tsTotal} 已下载:{downloaded.GetMemory()} - {data.Name} {url}");
                                } else {
                                    logger.info($"下载进度:{++tsCount}/{tsTotal}  已下载:{downloaded.GetMemory()} - {data.Name} {url}");
                                }
                                break;
                            }
                        }
                        if (!isSuccess) {
                            throw new System.Exception($"{data.Name} 下载失败 : {data.Urls}");
                        }
                        goto Start;
                    } catch (System.Exception e) {
                        return e.ToString();
                    }
                });
                tasks.Add(task);
            }
            File.WriteAllLines($"{tsBase}/file.m3u8", lines.ToArray());
            while (tasks.Count > 0) {
                var taskIndex = Task.WaitAny(tasks.ToArray());
                var taskResult = tasks[taskIndex].Result;
                tasks.RemoveAt(taskIndex);
                if (!string.IsNullOrEmpty(taskResult)) {
                    throw new System.Exception(taskResult);
                }
            }
            //Task.WaitAll(tasks.ToArray());
            //var fileList = new List<string>();
            //for (var i = 0; i < tsTotal; i++) {
            //    fileList.Add(string.Format("file '{0}'", Path.GetFullPath(string.Format("{0}/{1:00000}.ts", tsBase, i + 1))));
            //}
            //File.WriteAllLines($"{tsBase}/file.txt", fileList.ToArray());
            FileUtil.DeleteFile(output);
            FileUtil.DeleteFolder(output, null, true);
            logger.info("合并ts文件...");
            var exitCode = ExecuteFFmpeg("-f", "concat", "-safe", "0", "-i", $"{tsBase}/file.m3u8", "-vcodec", "copy", "-acodec", "copy", output);
            if (exitCode != 0) {
                throw new System.Exception($"FFmpeg 合并 ts 文件出错 : {exitCode}");
            }
            logger.info($"下载完成:{output}");
        }
    }
}