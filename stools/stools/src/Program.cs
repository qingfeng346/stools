﻿using System;
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
using Exception = System.Exception;
namespace Scorpio.stools {
    class Program {
        private readonly static string HelpAndroidpublisher = @"
    --auth|-auth                (必填)服务账号json文件
    --packageName|-packageName  (必填)App PackageName
    --version|-version          版本VersionCode
    --name|-name                轨道的名字
    --apk|-apk                  apk文件
    --obb|-obb                  obb文件,必须和apk一起上传
    --releasenote|-releasenote  ReleaseNote文件,xml文件
";
        private readonly static string HelpLookupMetadata = @"
    --username|-username|-u     (必填)服务账号
    --password|-password|-p     (必填)服务账号密码
    --appleid|-appleid|-id      (必填)AppID
    --output|-output|-o         导出目录,默认当前目录
";
        private readonly static string HelpUploadMetadata = @"
    --username|-username|-u     (必填)服务账号
    --password|-password|-p     (必填)服务账号密码
    --file|-file|-f             (必填)Metadata所在目录
";
        private readonly static string HelpLookupMobileprovision = @"
    --file|-file|-f             (必填)Mobileprovision文件路径
";
        private readonly static string HelpResign = @"
    --ipa|-ipa                  (必填)ipa原文件
    --provision|-provision|-p   (必填)符号文件
    --developer|-developer|-d   (必填)开发者名称
    --output|-output|-o         (必填)导出文件
";
        private readonly static string HelpWget = @"
    --url|-url                  (必填)下载链接
    --output|-output|-o         (必填)输出文件
";
        private readonly static string HelpDownloadMusic = @"
    --url|-url                  音乐详情链接
    --id|-id                    音乐ID
    --type|-type|-t             类型,默认kuwo 列表 kuwo(酷我) kugou(酷狗) cloud(网易云音乐)
    --output|-output|-o         导出目录,默认当前目录
";
        private readonly static string HelpDownloadAlbum = @"
    --url|-url                  专辑详情链接
    --id|-id                    专辑ID
    --type|-type|-t             类型,默认kuwo 列表 kuwo(酷我) cloud(网易云音乐)
    --namepath|-namepath        是否自动创建专辑文件夹
    --output|-output|-o         导出目录,默认当前目录
";
        private readonly static string HelpDownloadM3u8 = @"
    --url|-url                  (必填)M3U8地址
    --output|-output|-o         导出文件,具体文件路径
    --queue|-queue|-q           同时下载队列数量,默认16
";
        private readonly static string[] ParameterAuth = new [] { "--auth", "-auth" };
        private readonly static string[] ParameterPackageName = new [] { "--packageName", "-packageName" };
        private readonly static string[] ParameterVersion = new [] { "--version", "-version" };
        private readonly static string[] ParameterName = new [] { "--name", "-name" };
        private readonly static string[] ParameterApk = new [] { "--apk", "-apk" };
        private readonly static string[] ParameterObb = new [] { "--obb", "-obb" };
        private readonly static string[] ParameterReleasenote = { "--releasenote", "-releasenote" };
        private readonly static string[] ParameterUsername = { "--username", "-username", "-u" };
        private readonly static string[] ParameterPassword = { "--password", "-password", "-p" };
        private readonly static string[] ParameterAppleid = { "--appleid", "-appleid", "--apple_id", "-apple_id", "-id" };
        private readonly static string[] ParameterOutput = { "--output", "-output", "-o" };
        private readonly static string[] ParameterFile = { "--file", "-file", "-f" };
        private readonly static string[] ParameterType = { "--type", "-type", "-t" };
        private readonly static string[] ParameterID = { "--id", "-id" };
        private readonly static string[] ParameterUrl = { "--url", "-url" };
        private readonly static string[] ParameterNamePath = { "--namepath", "-namepath" };
        private readonly static string[] ParameterIpa = { "--ipa", "-ipa" };
        private readonly static string[] ParameterProvision = { "--provision", "-provision", "-p" };
        private readonly static string[] ParameterDeveloper = { "--developer", "-developer", "-d" };
        private readonly static string[] ParameterQueue = { "--queue", "-queue", "-q" };
        public class TSData {
            public int index;
            public List<string> urls = new List<string>();
            public string Name => string.Format("{0:00000}.ts", index);
            public string Urls => string.Join(";", urls);
        }
        static void Main (string[] args) {
            Encoding.RegisterProvider (CodePagesEncodingProvider.Instance);
            var perform = new Perform ();
            perform.AddExecute ("androidpublisher", "更新Google Play信息", HelpAndroidpublisher, Androidpublisher);
            perform.AddExecute ("lookupMetadata", "获取AppStore Metadata文件", HelpLookupMetadata, LookupMetadata);
            perform.AddExecute ("uploadMetadata", "更新AppStore Metadata文件", HelpUploadMetadata, UploadMetadata);
            perform.AddExecute ("lookupMobileprovision", "查看mobileprovision文件", HelpLookupMobileprovision, LookupMobileprovision);
            perform.AddExecute ("resign", "IOS ipa文件重签名", HelpResign, Resign);
            perform.AddExecute ("wget", "下载文件", HelpWget, Wget);
            perform.AddExecute ("downloadAlbum", "下载专辑", HelpDownloadAlbum, DownloadAlbum);
            perform.AddExecute ("downloadMusic", "下载音乐", HelpDownloadMusic, DownloadMusic);
            perform.AddExecute ("downloadM3u8", "下载M3U8文件", HelpDownloadM3u8, DownloadM3u8);
            try {
                Console.WriteLine($"stools : {Version.version}");
                perform.Start (args, null, null);
            } catch (System.Exception e) {
                Console.Error.WriteLine (e);
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
        static void Androidpublisher (Perform perform, CommandLine commandLine, string[] args) {
            var authFile = commandLine.GetValue (ParameterAuth);
            var packageName = commandLine.GetValue (ParameterPackageName);
            var versionCode = int.Parse (commandLine.GetValue (ParameterVersion));
            var apkFile = commandLine.GetValue (ParameterApk);
            var obbFile = commandLine.GetValue (ParameterObb);
            var name = commandLine.GetValue (ParameterName);
            var releaseNote = commandLine.GetValue (ParameterReleasenote);
            ExecuteAndroidpublisher (authFile, packageName, (service, editId) => {
                    if (!string.IsNullOrWhiteSpace (apkFile)) {
                        var apkSize = new FileInfo (apkFile).Length;
                        var apkUpload = service.Edits.Apks.Upload (packageName, editId, File.OpenRead (apkFile), "application/vnd.android.package-archive");
                        apkUpload.ProgressChanged += (progress) => {
                            Console.WriteLine ($"上传{apkFile}进度:{progress.BytesSent.GetMemory()}/{apkSize.GetMemory()}");
                        };
                        Console.WriteLine ($"开始上传apk文件:{apkFile}");
                        var apkResult = apkUpload.Upload ();
                        if (apkResult.Exception != null) {
                            throw apkResult.Exception;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace (obbFile)) {
                        var obbSize = new FileInfo (obbFile).Length;
                        var obbUpload = service.Edits.Expansionfiles.Upload (packageName, editId, versionCode, EditsResource.ExpansionfilesResource.UploadMediaUpload.ExpansionFileTypeEnum.Main, File.OpenRead (obbFile), "application/octet-stream");
                        obbUpload.ProgressChanged += (progress) => {
                            Console.WriteLine ($"上传{obbFile}进度:{progress.BytesSent.GetMemory()}/{obbSize.GetMemory()}");
                        };
                        Console.WriteLine ($"开始上传obb文件:{apkFile}");
                        var obbResult = obbUpload.Upload ();
                        if (obbResult.Exception != null) {
                            throw obbResult.Exception;
                        }
                    }
                },
                (release) => {
                    if (!string.IsNullOrWhiteSpace (name)) { release.Name = name; }
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
                    release.VersionCodes = new List<long?> () { versionCode };
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
        static void LookupMetadata (Perform perform, CommandLine commandLine, string[] args) {
            var username = commandLine.GetValue (ParameterUsername);
            var password = commandLine.GetValue (ParameterPassword);
            var id = commandLine.GetValue (ParameterAppleid);
            var output = commandLine.GetValueDefault (ParameterOutput, "./");
            ExecuteTMSTransporter (username, password, new [] { "-m", "lookupMetadata", "-apple_id", id, "-destination", output });
        }
        static void UploadMetadata (Perform perform, CommandLine commandLine, string[] args) {
            var username = commandLine.GetValue (ParameterUsername);
            var password = commandLine.GetValue (ParameterPassword);
            var file = commandLine.GetValue (ParameterFile);
            ExecuteTMSTransporter (username, password, new [] { "-m", "upload", "-f", file });
        }
        static void LookupMobileprovision (Perform perform, CommandLine commandLine, string[] args) {
            Console.WriteLine (GetMobileprovisionInfo (commandLine.GetValue (ParameterFile)));
        }
        static void Resign (Perform perform, CommandLine commandLine, string[] args) {
            var resign = "ios_resign_with_ipa.sh";
            var output = commandLine.GetValueDefault (ParameterOutput, "./");
            var provision = commandLine.GetValue (ParameterProvision);
            var developer = commandLine.GetValue (ParameterDeveloper);
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
            //FileUtil.CopyFile ($"{ScorpioUtil.BaseDirectory}/{resign}", resign, true);
            File.Copy ($"{ScorpioUtil.BaseDirectory}/{resign}", resign, true);
            ScorpioUtil.StartProcess ("sh", null, new string[] {
                resign,
                commandLine.GetValue (ParameterIpa),
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
        static void Wget (Perform perform, CommandLine commandLine, string[] args) {
            var urls = new List<string> ();
            urls.AddRange (commandLine.Args);
            urls.AddRange (commandLine.GetValues (ParameterUrl));
            var outputs = commandLine.GetValues (ParameterOutput);
            var tasks = new List<Task> ();
            for (var i = 0; i < urls.Count; i++) {
                var url = urls[i];
                var output = outputs.Length > i ? outputs[i] : GetFilenameByUrl(url);
                tasks.Add (Task.Run (async () => {
                    logger.info ($"开始下载 : {url}");
                    await HttpUtil.Download (url, output);
                    logger.info ($"下载完成 : {url}");
                }));
            }
            Task.WaitAll (tasks.ToArray ());
        }
        static void DownloadAlbum(Perform perform, CommandLine commandLine, string[] args) {
            var tasks = new List<Task>();
            var output = commandLine.GetValueDefault(ParameterOutput, "./");
            var urls = new List<string>();
            urls.AddRange(commandLine.Args);
            urls.AddRange(commandLine.GetValues(ParameterUrl));
            var createPath = commandLine.HadValue(ParameterNamePath);
            var ids = commandLine.GetValues(ParameterID);
            Task.WaitAll(Task.Run(async () => {
                if (ids.Length > 0) {
                    var music = MusicFactory.Create(commandLine.GetValueDefault(ParameterType, ""));
                    foreach (var id in ids) {
                        var albumInfo = await music.ParseAlbum(id);
                        var outputPath = createPath ? Path.Combine(output, albumInfo.name) : output;
                        foreach (var musicid in albumInfo.musicList) {
                            await music.Download(musicid, outputPath);
                        }
                    }
                }
                foreach (var url in urls) {
                    await DownloadAlbumUrl(url, output, createPath);
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
        static void DownloadMusic (Perform perform, CommandLine commandLine, string[] args) {
            var tasks = new List<Task> ();
            var output = commandLine.GetValueDefault (ParameterOutput, "./");
            var urls = new List<string> ();
            urls.AddRange (commandLine.Args);
            urls.AddRange (commandLine.GetValues (ParameterUrl));
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
        static void DownloadM3u8 (Perform perform, CommandLine commandLine, string[] args) {
            var url = commandLine.Args.Count > 0 ? commandLine.Args[0] : null;
            if (string.IsNullOrEmpty(url)) {
                url = commandLine.GetValue(ParameterUrl);
            }
            if (string.IsNullOrEmpty(url)) {
                throw new System.Exception("m3u8 url 不能为空");
            }
            var output = Path.GetFullPath(commandLine.GetValueDefault(ParameterOutput, FileUtil.GetMD5FromString(url) + ".mp4"));
            int queueCount = 8;
            if (int.TryParse(commandLine.GetValue(ParameterQueue), out var queuePar)) {
                queueCount = Math.Max(1, queuePar);
            }
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
            for (var i = 0; i < queueCount; i++) {
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