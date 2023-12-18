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
using Newtonsoft.Json;

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
        private const string ParameterFile = "--file|-file|-f";
        private const string ParameterType = "--type|-type|-t";
        private const string ParameterInfo = "--info|-info";
        private const string ParameterUrl = "--url|-url";
        private const string ParameterIpa = "--ipa|-ipa";
        private const string ParameterProvision = "--provision|-provision|-p";
        private const string ParameterDeveloper = "--developer|-developer|-d";
        private const string ParameterQueue = "--queue|-queue|-q";
        static void Main(string[] args) {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var perform = new Perform();
            perform.AddExecute("androidpublisher", "更新Google Play信息", Androidpublisher);
            perform.AddExecute("googledrivedownload", "", GoogleDriveDownload);
            perform.AddExecute("lookupMobileprovision", "查看mobileprovision文件", LookupMobileprovision);
            perform.AddExecute("resign", "ipa文件重签名", Resign);
            perform.AddExecute("wget", "下载文件", Wget);
            perform.AddExecute("downloadM3u8", "下载M3U8文件", DownloadM3u8);
            perform.AddExecute("sortmedia", "整理图片和视频", SortMedia);
            perform.AddExecute("md5", "获取文件MD5", GetMD5);
            perform.AddExecute("musicInfo", "设置音频信息", MusicInfo);
            try {
                perform.Start(args);
                Environment.Exit(0);
            } catch (System.Exception e) {
                Console.Error.WriteLine(e);
                Environment.Exit(1);
            }
        }
        static void Androidpublisher([ParamterInfo("服务账号json文件", ParameterAuth)] string auth,
                                     [ParamterInfo("PackageName", ParameterPackageName)] string packageName,
                                     [ParamterInfo("VersionCode", ParameterVersion, false)] int version,
                                     [ParamterInfo("轨道名字", ParameterTrackName, false)] string trackName,
                                     [ParamterInfo("Aab文件", ParameterAab, false)] string aab,
                                     [ParamterInfo("Apk文件", ParameterApk, false)] string apk,
                                     [ParamterInfo("Obb文件", ParameterObb, false)] string obb,
                                     [ParamterInfo("ReleaseNote文件(xml文件)", ParameterReleasenote, false)] string releaseNote) {
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
        static void GoogleDriveDownload([ParamterInfo("服务账号json文件", ParameterAuth)] string auth,
                                        [ParamterInfo("文件ID", ParameterFileId)] string fileId,
                                        [ParamterInfo("文件MimeType", ParameterMimeType)] string mimeType,
                                        [ParamterInfo("保存路径", ParameterOutput)] string file) {
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
        static void LookupMobileprovision([ParamterInfo("文件", ParameterFile)] string file) {
            Console.WriteLine(Util.GetMobileprovisionInfo(file));
        }
        static void Resign([ParamterInfo("ipa文件", ParameterIpa)] string ipa,
                           [ParamterInfo("描述文件", ParameterProvision)] string provision,
                           [ParamterInfo("开发者名称", ParameterDeveloper)] string developer,
                           [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
            var resign = "ios_resign_with_ipa.sh";
            if (string.IsNullOrEmpty(developer)) {
                var mobileprovisionInfo = Util.GetMobileprovisionInfo(provision);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(mobileprovisionInfo);
                var dict = xmlDoc.DocumentElement["dict"];
                foreach (XmlNode node in dict) {
                    if (node.Name == "key" && node.InnerText == "TeamName") {
                        developer = $"iPhone Distribution: {node.NextSibling.InnerText}";
                        break;
                    }
                }
            }
            File.Copy($"{ScorpioUtil.BaseDirectory}/{resign}", resign, true);
            ScorpioUtil.Execute("sh", null, new string[] {
                resign,
                ipa,
                developer,
                provision,
                output
            });
            FileUtil.DeleteFile(resign);
        }
        static void Wget(CommandLine commandLine,
                         [ParamterInfo("下载URL", ParameterUrl, false)] string[] url,
                         [ParamterInfo("输出目录", ParameterOutput, "./", false)] string output) {
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
        static void DownloadM3u8(CommandLine commandLine,
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
                throw new System.Exception($"{data.Name} 下载失败 : {builder.ToString()}");
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
        static void SortMedia([ParamterInfo("类型,0 同步 1 去重", ParameterType, "0", false)] int type,
                              [ParamterInfo("其实目录", "--source|-source")] string source,
                              [ParamterInfo("目标目录", "--target|-target")] string target) {
            if (type == 0) {
                MediaUtil.Sync(source, target);
            } else if (type == 1) {
                MediaUtil.Distinct(source, target);
            }
        }
        static void GetMD5(CommandLine commandLine) {
            foreach (var file in commandLine.Args) {
                logger.info(FileUtil.GetMD5FromFile(file));
            }
        }
        static void MusicInfo([ParamterInfo("音频文件", ParameterFile)] string file,
                              [ParamterInfo("音频信息", ParameterInfo)] string info) {
            MusicUtil.SetMusicInfo(file, JsonConvert.DeserializeObject<MusicInfo>(FileUtil.GetFileString(info)));
        }
    }
}