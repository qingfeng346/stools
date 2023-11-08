using System;
using System.Collections.Generic;
using Scorpio.Commons;
using System.IO;
using System.Globalization;

namespace Scorpio.stools {
    public class MediaUtil {
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
    }
}