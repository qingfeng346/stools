using System;
using Scorpio.Commons;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Tomlyn;
namespace stools {
    class Program {
        private readonly static string[] ParameterAuth = new[] { "--auth", "-auth" };
        private readonly static string[] ParameterPackageName = new[] { "--packageName", "-packageName" };
        private readonly static string[] ParameterVersion = new[] { "--version", "-version" };
        private readonly static string[] ParameterName = new[] { "--name", "-name" };
        private readonly static string[] ParameterApk = new[] { "--apk", "-apk" };
        private readonly static string[] ParameterObb = new[] { "--obb", "-obb" };
        private readonly static string[] ParameterReleasenote = { "--releasenote", "-releasenote" };
        static void Main(string[] args) {
            var perform = new Perform();
            perform.AddExecute("androidpublisher", "", Androidpublisher);
            perform.AddExecute("uploadipa", "", UploadIpa);
            try {
                perform.Start(args, null, null);
            } catch (System.Exception e) {
                Console.Error.WriteLine(e);
            }
        }
        static void ExecuteAndroidpublisher(string authFile, string packageName, Action<AndroidPublisherService, string> action, Action<TrackRelease> trackAction) {
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
        static void Androidpublisher(Perform perform, CommandLine commandLine, string[] args) {
            var authFile = commandLine.GetValue(ParameterAuth);
            var packageName = commandLine.GetValue(ParameterPackageName);
            var versionCode = int.Parse(commandLine.GetValue(ParameterVersion));
            var apkFile = commandLine.GetValue(ParameterApk);
            var obbFile = commandLine.GetValue(ParameterObb);
            var name = commandLine.GetValue(ParameterName);
            var releaseNote = commandLine.GetValue(ParameterReleasenote);
            ExecuteAndroidpublisher(authFile, packageName, (service, editId) => {
                if (!string.IsNullOrWhiteSpace(apkFile)) {
                    var apkSize = new FileInfo(apkFile).Length;
                    var apkUpload = service.Edits.Apks.Upload(packageName, editId, File.OpenRead(apkFile), "application/vnd.android.package-archive");
                    apkUpload.ProgressChanged += (progress) => {
                        Console.WriteLine($"上传{apkFile}进度:{progress.BytesSent.GetMemory()}/{apkSize.GetMemory()}");
                    };
                    Console.WriteLine($"开始上传apk文件:{apkFile}");
                    var apkResult = apkUpload.Upload();
                    if (apkResult.Exception != null) {
                        throw apkResult.Exception;
                    }
                }
                if (!string.IsNullOrWhiteSpace(obbFile)) {
                    var obbSize = new FileInfo(obbFile).Length;
                    var obbUpload = service.Edits.Expansionfiles.Upload(packageName, editId, versionCode, EditsResource.ExpansionfilesResource.UploadMediaUpload.ExpansionFileTypeEnum.Main, File.OpenRead(obbFile), "application/octet-stream");
                    obbUpload.ProgressChanged += (progress) => {
                        Console.WriteLine($"上传{obbFile}进度:{progress.BytesSent.GetMemory()}/{obbSize.GetMemory()}");
                    };
                    Console.WriteLine($"开始上传obb文件:{apkFile}");
                    var obbResult = obbUpload.Upload();
                    if (obbResult.Exception != null) {
                        throw obbResult.Exception;
                    }
                }
            },
            (release) => {
                if (!string.IsNullOrWhiteSpace(name)) { release.Name = name; }
                if (!string.IsNullOrWhiteSpace(releaseNote)) {
                    release.ReleaseNotes = new List<LocalizedText>();
                    var model = Toml.ToModel(FileUtil.GetFileString(releaseNote));
                    foreach (var pair in model) {
                        release.ReleaseNotes.Add(new LocalizedText() {
                            Language = pair.Key,
                            Text = (string)pair.Value
                        });
                    }
                }
                release.VersionCodes = new List<long?>() { versionCode };
            });
        }
        static void UploadIpa(Perform perform, CommandLine commandLine, string[] args) {

        }
    }
}
