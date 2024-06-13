using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Drive.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Scorpio.Commons;

namespace Scorpio.stools {
    partial class Program {
        private const string ParameterAuth = "--auth|-auth";
        private const string ParameterPackageName = "--packageName|-packageName";
        private const string ParameterVersion = "--version|-version";
        private const string ParameterTrackName = "--trackName|-trackName";
        private const string ParameterAab = "--aab|-aab";
        private const string ParameterApk = "--apk|-apk";
        private const string ParameterObb = "--obb|-obb";
        private const string ParameterReleasenote = "--releasenote|-releasenote|--releaseNote|-releaseNote";
        private const string ParameterOutput = "--output|-output|-o";
        private const string ParameterFileId = "--fileId|-fileId";
        private const string ParameterMimeType = "--mimeType|-mimeType";
        private const string ParameterUrl = "--url|-url";
        private const string ParameterQueue = "--queue|-queue|-q";
        private const string ParameterSource = "--source|-source|-s";
        private const string ParameterClear = "--clear|-clear|-c";
        private const string ParameterSort = "--sort|-sort";
        private const string ParameterMove = "--move|-move";
        unsafe static void Main(string[] args) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            logger.SetLogger(new ConsoleLogger());
            var perform = new Perform();
            perform.AddExecute("androidpublisher", "更新Google Play信息", Androidpublisher);
            perform.AddExecute("googledrivedownload", "", GoogleDriveDownload);
            perform.AddExecute("wget", "下载文件", Wget);
            perform.AddExecute("downloadM3u8", "下载M3U8文件", DownloadM3u8);
            perform.AddExecute("downloadmusic", "下载音乐", DownloadMusic);
            perform.AddExecute("md5", "获取文件MD5", GetMD5);
            perform.AddExecute("sortmedia", "整理图片和视频", SortMedia);
            perform.AddExecute("sortmusic", "整理音频", SortMusic);
            try {
                perform.Start(args);
                Environment.Exit(0);
            } catch (System.Exception e) {
                Console.Error.WriteLine(e);
                Environment.Exit(1);
            }
        }
        static void Androidpublisher([ParamterInfo(Label = "服务账号json文件", Param = ParameterAuth, Required = true)] string auth,
                                     [ParamterInfo(Label = "PackageName", Param = ParameterPackageName, Required = true)] string packageName,
                                     [ParamterInfo(Label = "VersionCode", Param = ParameterVersion)] int version,
                                     [ParamterInfo(Label = "轨道名字", Param = ParameterTrackName)] string trackName,
                                     [ParamterInfo(Label = "Aab文件", Param = ParameterAab)] string aab,
                                     [ParamterInfo(Label = "Apk文件", Param = ParameterApk)] string apk,
                                     [ParamterInfo(Label = "Obb文件", Param = ParameterObb)] string obb,
                                     [ParamterInfo(Label = "ReleaseNote文件(xml文件)", Param = ParameterReleasenote)] string releaseNote) {
            Util.ExecuteAndroidpublisher(auth, packageName,
                (service, editId) => {
                    if (!string.IsNullOrWhiteSpace(aab)) {
                        var aabSize = new FileInfo(aab).Length;
                        var aabUpload = service.Edits.Apks.Upload(packageName, editId, File.OpenRead(aab), "application/vnd.android.package-archive");
                        aabUpload.ProgressChanged += (progress) => {
                            Console.WriteLine($"上传{aab}进度:{progress.BytesSent.GetMemory()}/{aabSize.GetMemory()}");
                        };
                        Console.WriteLine($"开始上传apk文件:{aab}");
                        var aabResult = aabUpload.Upload();
                        if (aabResult.Exception != null) {
                            throw aabResult.Exception;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(apk)) {
                        var apkSize = new FileInfo(apk).Length;
                        var apkUpload = service.Edits.Apks.Upload(packageName, editId, File.OpenRead(apk), "application/vnd.android.package-archive");
                        apkUpload.ProgressChanged += (progress) => {
                            Console.WriteLine($"上传{apk}进度:{progress.BytesSent.GetMemory()}/{apkSize.GetMemory()}");
                        };
                        Console.WriteLine($"开始上传apk文件:{apk}");
                        var apkResult = apkUpload.Upload();
                        if (apkResult.Exception != null) {
                            throw apkResult.Exception;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(obb)) {
                        var obbSize = new FileInfo(obb).Length;
                        var obbUpload = service.Edits.Expansionfiles.Upload(packageName, editId, version, EditsResource.ExpansionfilesResource.UploadMediaUpload.ExpansionFileTypeEnum.Main, File.OpenRead(obb), "application/octet-stream");
                        obbUpload.ProgressChanged += (progress) => {
                            Console.WriteLine($"上传{obb}进度:{progress.BytesSent.GetMemory()}/{obbSize.GetMemory()}");
                        };
                        Console.WriteLine($"开始上传obb文件:{obb}");
                        var obbResult = obbUpload.Upload();
                        if (obbResult.Exception != null) {
                            throw obbResult.Exception;
                        }
                    }
                },
            (release) => {
                if (!string.IsNullOrWhiteSpace(trackName)) { release.Name = trackName; }
                if (!string.IsNullOrWhiteSpace(releaseNote)) {
                    release.ReleaseNotes = new List<LocalizedText>();
                    var xmlDoc = new XmlDocument();
                    xmlDoc.Load(releaseNote);
                    foreach (XmlNode language in xmlDoc.DocumentElement) {
                        release.ReleaseNotes.Add(new LocalizedText() {
                            Language = language.Name,
                            Text = language.InnerText.Trim()
                        });
                    }
                }
                if (version > 0)
                    release.VersionCodes = new List<long?>() { version };
            });
        }
        static void GoogleDriveDownload([ParamterInfo(Label = "服务账号json文件", Param = ParameterAuth, Required = true)] string auth,
                                        [ParamterInfo(Label = "文件ID", Param = ParameterFileId, Required = true)] string fileId,
                                        [ParamterInfo(Label = "文件MimeType", Param = ParameterMimeType, Required = true)] string mimeType,
                                        [ParamterInfo(Label = "保存路径", Param = ParameterOutput, Required = true)] string file) {
            var service = new DriveService(new BaseClientService.Initializer() {
                HttpClientInitializer = GoogleCredential.FromFile(auth).CreateScoped("https://www.googleapis.com/auth/drive"),
            });
            var request = service.Files.Export(fileId, mimeType);
            FileUtil.CreateDirectoryByFile(file);
            var error = "";
            request.MediaDownloader.ProgressChanged += (progress) => {
                if (progress.Status == Google.Apis.Download.DownloadStatus.Downloading) {
                    Console.WriteLine($"下载{file}进度:{progress.BytesDownloaded.GetMemory()}");
                } else if (progress.Status == Google.Apis.Download.DownloadStatus.Completed) {
                    Console.WriteLine($"下载{file}完成,总大小:{progress.BytesDownloaded.GetMemory()}");
                } else if (progress.Status == Google.Apis.Download.DownloadStatus.Failed) {
                    error = $"下载{file}失败:{progress.Exception}";
                    Console.WriteLine(error);
                }
            };
            FileUtil.DeleteFile(file);
            using (var stream = new FileStream(file, FileMode.CreateNew)) {
                request.Download(stream);
            }
            if (!string.IsNullOrEmpty(error)) {
                throw new System.Exception(error);
            }
        }
        static void Wget(CommandLine commandLine,
                         [ParamterInfo(Label = "下载URL", Param = ParameterUrl)] string[] url,
                         [ParamterInfo(Label = "输出目录", Param = ParameterOutput, Default = "./")] string output) {
            var urls = new List<string>();
            urls.AddRange(commandLine.Args);
            if (url != null) urls.AddRange(url);
            var tasks = new List<Task>();
            for (var i = 0; i < urls.Count; i++) {
                var uri = urls[i];
                tasks.Add(Task.Run(async () => {
                    logger.info($"开始下载 : {uri}");
                    var fileName = Path.Combine(output, Util.GetFilenameByUrl(uri));
                    var response = await HttpUtil.Download(uri, fileName);
                    if (response.IsSuccessStatusCode)
                        logger.info($"下载完成 : {fileName}");
                    else
                        logger.info($"下载失败 {fileName} : {response.StatusCode}({(int)response.StatusCode}) - {response.Error}");
                    
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }
        static void GetMD5(CommandLine commandLine) {
            foreach (var file in commandLine.Args) {
                if (FileUtil.FileExist(file)) {
                    logger.info(FileUtil.GetMD5FromFile(file));
                } else {
                    logger.info(FileUtil.GetMD5FromString(file));
                }
            }
        }
        static void DownloadM3u8(CommandLine commandLine,
                                  [ParamterInfo(Label = "M3U8链接", Param = ParameterUrl)] string url,
                                  [ParamterInfo(Label = "下载队列数", Param = ParameterQueue, Default = "8")] int queue,
                                  [ParamterInfo(Label = "输出文件", Param = ParameterOutput)] string output) {
            if (string.IsNullOrEmpty(url)) {
                url = commandLine.Args.Count > 0 ? commandLine.Args[0] : null;
            }
            if (string.IsNullOrEmpty(url)) {
                throw new System.Exception("m3u8 url 不能为空");
            }
            if (string.IsNullOrEmpty(output)) {
                output = Path.GetFullPath(FileUtil.GetMD5FromString(url));
            }
            queue = Math.Max(1, queue);
            string m3u8Content = "";
            Task.WaitAll(Task.Run(async () => { m3u8Content = await HttpUtil.Get(url); }));
            var baseUrl = url.Substring(0, url.IndexOf("/", url.StartsWith("http") ? 7 : 8));
            var parentUrl = url.Substring(0, url.LastIndexOf("/") + 1);
            var lines = m3u8Content.Split("\n");
            var index = 1;
            var tsBase = $"{output}";
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
            var tsTotal = tsList.Count;
            var downloaded = 0L;
            ScorpioUtil.StartQueue(tsList, async (data, index) => {
                var builder = new StringBuilder();
                for (var i = 0; i < 5; ++i) {
                    builder.Clear();
                    foreach (var url in data.urls) {
                        var result = await HttpUtil.Download(url, $"{tsBase}/{data.Name}", false, true);
                        if (result.IsSuccessStatusCode) {
                            downloaded += result.Length;
                            if (result.Skip) {
                                logger.info($"[跳过] 下载进度:{++tsCount}/{tsTotal} 已下载:{downloaded.GetMemory()} - {data.Name} {url}");
                            } else {
                                logger.info($"下载进度:{++tsCount}/{tsTotal}  已下载:{downloaded.GetMemory()} - {data.Name} {url}");
                            }
                            return;
                        } else {
                            builder.AppendLine($"{url} code : {result.StatusCode}  error : {result.Error}");
                        }
                    }
                }
                throw new System.Exception($"{data.Name} 下载失败 : {builder}");
            }, queue);
            File.WriteAllLines($"{tsBase}/file.m3u8", lines.ToArray());
            FileUtil.DeleteFile(output);
            // FileUtil.DeleteFolder(output, null, true);
            logger.info("合并ts文件...");
            var exitCode = Util.ExecuteFFmpeg("-f", "concat", "-safe", "0", "-i", $"{tsBase}/file.m3u8", "-vcodec", "copy", "-acodec", "copy", $"{output}.mp4");
            if (exitCode != 0) {
                throw new System.Exception($"FFmpeg 合并 ts 文件出错 : {exitCode}");
            }
            FileUtil.DeleteFolder(tsBase);
            logger.info($"下载完成:{output}");
        }
        static void SortMedia([ParamterInfo(Label = "起始目录", Param = ParameterSource)] string source,
                              [ParamterInfo(Label = "输出目录", Param = ParameterOutput)] string target,
                              [ParamterInfo(Label = "清理目录", Param = ParameterClear, Required = false)] bool clear) {
            MediaUtil.SortMedia(source, target, clear);
        }
        static void SortMusic([ParamterInfo(Label = "起始目录", Param = ParameterSource)] string source,
                              [ParamterInfo(Label = "输出目录", Param = ParameterOutput)] string target, 
                              [ParamterInfo(Label = "清理目录", Param = ParameterClear, Required = false)] bool clear,
                              [ParamterInfo(Label = "移动文件", Param = ParameterMove, Required = false)] bool move) {
            MusicUtil.SortMusic(source, target, clear, move);
        }
        static void DownloadMusic(CommandLine commandLine, 
                                  [ParamterInfo(Label = "音乐地址", Param = ParameterUrl)] string[] url,
                                  [ParamterInfo(Label = "输出目录", Param = ParameterOutput)] string target) {
            var urls = new List<string>();
            urls.AddRange(commandLine.Args);
            if (url != null) urls.AddRange(url);
            Task.Run(() => MusicUtil.DownloadMusicUrls(urls.ToArray(), target, MusicPath.None)).Wait();
        }
    }
}