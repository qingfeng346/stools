using System;
using System.Collections.Generic;
using Scorpio.Commons;
using Newtonsoft.Json;
using System.IO;
using System.Globalization;

namespace Scorpio.stools {
    public class MediaUtil {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        private const string FileNameFormat = "yyyyMMdd_HHmmss_fff";
        public class DistinctFile {
            public string file;
            public bool isBackup;
        }
        public static void Sync(string source, string target) {
            var dateTimes = new HashSet<DateTime>();
            if (Directory.Exists(target)) {
                foreach (var file in Directory.GetFiles(target, "*", SearchOption.AllDirectories)) {
                    var name = Path.GetFileNameWithoutExtension(file);
                    dateTimes.Add(DateTime.ParseExact(name, FileNameFormat, CultureInfo.InvariantCulture));
                }
            }
            var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            var total = files.Length;
            var progress = new Progress(total);
            for (var i = 0; i < total; ++i) {
                progress.SetProgress(i);
                var file = files[i];
                var mediaInfo = Util.GetMediaInfo(file);
                if (mediaInfo == null) {
                    logger.info($"{file} 文件不是有效的媒体文件");
                    continue;
                }
                var dateTime = mediaInfo.createTime;
                while (true) {
                    if (!dateTimes.Contains(dateTime)) {
                        dateTimes.Add(dateTime);
                        break;
                    }
                    dateTime = dateTime.AddMilliseconds(1);
                }
                var name = dateTime.ToString(FileNameFormat);
                var extension = Path.GetExtension(file);
                var mediaType = mediaInfo.isImage ? "照片" : "视频";
                var targetFile = $"{target}/{mediaType}/{dateTime.Year}/{name}{extension}";
                FileUtil.CreateDirectoryByFile(targetFile);
                File.Move(file, targetFile);
            }
            FileUtil.DeleteEmptyFolder(source, true);
        }
        public static void Distinct(string source, string backup) {
            var hashFiles = new Dictionary<MediaInfo, DistinctFile>();
            var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            var total = files.Length;
            var progress = new Progress(total);
            for (var i = 0; i < total; i++) {
                progress.SetProgress(i);
                var file = files[i];
                var mediaInfo = Util.GetMediaInfo(file);
                if (mediaInfo == null) {
                    logger.info($"{file} 文件不是有效的媒体文件");
                    continue;
                }
                if (!hashFiles.TryGetValue(mediaInfo, out var distinctFile)) {
                    hashFiles.Add(mediaInfo, new DistinctFile() { file = file });
                    continue;
                }
                if (!FileUtil.CompareFile(distinctFile.file, file)) {
                    continue;
                }
                var pathName = (mediaInfo.isImage ? "图片/" : "视频/") + Path.GetFileNameWithoutExtension(distinctFile.file);
                var extension = Path.GetExtension(distinctFile.file);
                if (!distinctFile.isBackup) {
                    distinctFile.isBackup = true;
                    FileUtil.CopyFile(distinctFile.file, $"{backup}/{pathName}/{Guid.NewGuid()}{extension}");
                }
                FileUtil.MoveFile(file, $"{backup}/{pathName}/{Guid.NewGuid()}{extension}", true);
                logger.info($"发现重复文件 {file} -> {distinctFile.file}");
            }
        }
        //public static void CreateFiles(string source) {
        //    var errorFiles = new List<string>();
        //    var hashFiles = new Dictionary<MediaInfo, List<MediaInfo>>();
        //    var repeatFiles = new Dictionary<MediaInfo, List<MediaInfo>>();
        //    FileUtil.DeleteFolder($"{source}/infos");
        //    var files = new Queue<string>(Directory.GetFiles(source, "*", SearchOption.AllDirectories));
        //    var total = files.Count;
        //    var count = 0;
        //    var sync = new object();
        //    Action<string> progress = (message) => {
        //        ++count;
        //        var p = string.Format("{0:00.00}", (count * 100f) / total);
        //        logger.info($"进度:{count}/{total}({p}%) {message}");
        //    };
        //    Util.StartQueue(1, files, async (file) => {
        //        var mediaInfo = Util.GetMediaInfo(file);
        //        if (mediaInfo == null) {
        //            errorFiles.Add(file);
        //            progress($"{file} 文件不是有效的媒体文件");
        //            return;
        //        }
        //        lock (sync) {
        //            if (!hashFiles.TryGetValue(mediaInfo, out var list)) {
        //                hashFiles[mediaInfo] = list = new List<MediaInfo>();
        //            }
        //            list.Add(mediaInfo);
        //            if (list.Count > 1) {
        //                progress($"{file} 发现相同文件 {list[0].targetFile}");
        //                return;
        //            }
        //        }
        //        progress($"{file}");
        //    });
        //    foreach (var pair in hashFiles) {
        //        if (pair.Value.Count > 1) {
        //            repeatFiles[pair.Key] = pair.Value;
        //        }
        //    }
        //    FileUtil.CreateFile($"{source}/infos/files.json", JsonConvert.SerializeObject(hashFiles, Formatting.Indented, serializerSettings));
        //    FileUtil.CreateFile($"{source}/infos/error.json", JsonConvert.SerializeObject(errorFiles, Formatting.Indented, serializerSettings));
        //    FileUtil.CreateFile($"{source}/infos/repeat.json", JsonConvert.SerializeObject(repeatFiles, Formatting.Indented, serializerSettings));
        //}

    }
}