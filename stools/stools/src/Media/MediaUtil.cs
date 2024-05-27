using Scorpio.Commons;
using System;
using System.Collections.Generic;
using System.IO;

namespace Scorpio.stools {
    public class MediaUtil {
        private const string FileNameFormat = "yyyyMMdd_HHmmss";
        private const string RepeatPathFormat = "yyyy/MM/dd/HHmmss";
        private const string PathFormat = "yyyy/MM/dd";
        public class DistinctFile {
            public string file;
            public bool isBackup;
        }

        static string GetFileName(string path, DateTime? dateTime, string extension) {
            while (true) {
                var file = $"{path}{dateTime.Value.ToString(FileNameFormat)}{extension}";
                if (!File.Exists(file))
                    return file;
                dateTime = dateTime.Value.AddSeconds(1);
            }
        }
        public static void SortMedia(string source, string target, bool sortExist, bool clear) {
            var distinctFiles = new Dictionary<MediaInfo, string>();
            if (clear) FileUtil.DeleteFolder(target);
            var originFileCount = 0;
            if (FileUtil.PathExist($"{target}/整理文件")) {
                var files = FileUtil.GetFiles($"{target}/整理文件", "*", SearchOption.AllDirectories);
                var progress = new Progress(files.Count, "获取文件");
                long imageSize = 0;
                long imageCount = 0;
                long mediaSize = 0;
                long mediaCount = 0;
                for (var i = 0; i < files.Count; ++i) {
                    var file = files[i];
                    var mediaInfo = Util.GetMediaInfo(file);
                    distinctFiles[mediaInfo] = file;
                    if (mediaInfo.isImage) {
                        imageSize += mediaInfo.size;
                        imageCount++;
                    } else {
                        mediaSize += mediaInfo.size;
                        mediaCount++;
                    }
                    originFileCount++;
                    progress.SetProgress(i, () => {
                        return $"总文件数量:{originFileCount} 图片数量:{imageCount},大小:{ScorpioUtil.GetMemory(imageSize)} 视频数量:{mediaCount},大小:{ScorpioUtil.GetMemory(mediaSize)}";
                    });
                }
                logger.info($"总文件数量:{originFileCount},有效文件数量:{distinctFiles.Count} 图片数量:{imageCount},大小:{ScorpioUtil.GetMemory(imageSize)} 视频数量:{mediaCount},大小:{ScorpioUtil.GetMemory(mediaSize)}");
            }
            if (sortExist) {
                var progress = new Progress(distinctFiles.Count, "整理文件");
                var i = 0;
                var newFiles = new Dictionary<MediaInfo, string>();
                foreach (var pair in distinctFiles) {
                    progress.SetProgress(i++);
                    var mediaInfo = pair.Key;
                    var dateTime = mediaInfo.createTime;
                    var timeError = mediaInfo.isTime ? "有效时间" : "无效时间";
                    var mediaType = mediaInfo.isImage ? "照片" : "视频";
                    var targetFile = GetFileName($"{target}/整理文件Backup/{timeError}/{mediaType}/{dateTime.Value.ToString(PathFormat)}/", dateTime, Path.GetExtension(pair.Value));
                    FileUtil.MoveFile(pair.Value, targetFile);
                    newFiles[pair.Key] = targetFile.Replace("/整理文件Backup/", "/整理文件/");
                }
                distinctFiles = newFiles;
                FileUtil.MoveFiles($"{target}/整理文件Backup", $"{target}/整理文件", "*", true, true);
            }
            {
                var files = FileUtil.GetFiles(source, "*", SearchOption.AllDirectories);
                var progress = new Progress(files.Count, "复制文件");
                var invalidCount = 0;
                var repeatCount = 0;
                var validCount = 0;
                for (var i = 0; i < files.Count; ++i) {
                    progress.SetProgress(i);
                    var file = files[i];
                    var mediaInfo = Util.GetMediaInfo(file);
                    if (mediaInfo == null) {
                        invalidCount++;
                        var errorPath = $"{target}/错误文件/";
                        var fileName = Path.GetFileName(file);
                        var errorFilePath = $"{errorPath}/{fileName}";
                        var index = 1;
                        while (FileUtil.FileExist(errorFilePath)) {
                            errorFilePath = $"{errorPath}/{fileName}.{index++}";
                        }
                        FileUtil.CopyFile(file, errorFilePath, true);
                        FileUtil.CreateFile($"{errorFilePath}.txt", file);
                        continue;
                    }
                    if (distinctFiles.TryGetValue(mediaInfo, out var origin)) {
                        repeatCount++;
                        var dateTime = mediaInfo.createTime;
                        var extension = Path.GetExtension(file);
                        var timeError = mediaInfo.isTime ? "有效时间" : "无效时间";
                        var mediaType = mediaInfo.isImage ? "重复照片" : "重复视频";
                        var targetPath = $"{target}/重复文件/{timeError}/{mediaType}/{dateTime.Value.ToString(RepeatPathFormat)}/{mediaInfo.md5}_{mediaInfo.size}/";
                        var targetFile = GetFileName(targetPath, dateTime, extension);
                        FileUtil.CopyFile(file, targetFile, true);
                        if (!FileUtil.FileExist($"{targetPath}/info.txt")) {
                            FileUtil.CreateFile($"{targetPath}/info.txt", origin);
                        }
                        if (!FileUtil.FileExist($"{targetPath}/origin{extension}")) {
                            FileUtil.CopyFile(origin, $"{targetPath}/origin{extension}", true);
                        }
                    } else {
                        validCount ++;
                        var dateTime = mediaInfo.createTime;
                        var timeError = mediaInfo.isTime ? "有效时间" : "无效时间";
                        var mediaType = mediaInfo.isImage ? "照片" : "视频";
                        var targetFile = GetFileName($"{target}/整理文件/{timeError}/{mediaType}/{dateTime.Value.ToString(PathFormat)}/", dateTime, Path.GetExtension(file));
                        distinctFiles[mediaInfo] = targetFile;
                        FileUtil.CopyFile(file, targetFile, true);
                    }
                }
                logger.info($"总文件数量:{files.Count},成功文件:{validCount},重复文件:{repeatCount},无效文件:{invalidCount}");
                logger.info($"目前文件总数量:{validCount + originFileCount}");
            }
        }
        
        
    }
}