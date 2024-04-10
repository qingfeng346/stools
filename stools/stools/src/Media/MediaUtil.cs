using System;
using System.Collections.Generic;
using Scorpio.Commons;
using System.IO;
using System.Runtime.ConstrainedExecution;

namespace Scorpio.stools {
    public class MediaUtil {
        private const string FileNameFormat = "yyyyMMdd_HHmmss";
        private const string PathFormat = "yyyy/MM";
        public class DistinctFile {
            public string file;
            public bool isBackup;
        }
        public class Album {
            public class Data {
                public string singer;
                public HashSet<string> performers;
                public string path;
                public List<string> files = new List<string>();
            }
            public List<Data> datas = new List<Data>();
            public Data GetData(string singer) {
                return datas.Find(x => x.singer == singer);
            }
        }
        static string GetFileName(string path, DateTime dateTime, string extension) {
            while (true) {
                var file = $"{path}{dateTime.ToString(FileNameFormat)}{extension}";
                if (!File.Exists(file))
                    return file;
                dateTime = dateTime.AddSeconds(1);
            }
        }
        public static void SortMedia(string source, string target, bool clear) {
            var distinctFiles = new Dictionary<MediaInfo, string>();
            var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            var total = files.Length;
            if (clear) FileUtil.DeleteFolder(target);
            var progress = new Progress(total);
            FileUtil.CreateFile($"{target}/number.txt", $"总数量 : {total}");
            for (var i = 0; i < total; ++i) {
                progress.SetProgress(i);
                var file = files[i];
                var mediaInfo = Util.GetMediaInfo(file);
                if (mediaInfo == null) {
                    var errorPath = $"{target}/错误文件/";
                    var count = Directory.Exists(errorPath) ? Directory.GetFiles(errorPath).Length : 0;
                    FileUtil.CopyFile(file, $"{errorPath}{count}{Path.GetExtension(file)}", true);
                    logger.info($"{file} 文件不是有效的媒体文件");
                    continue;
                }
                if (distinctFiles.TryGetValue(mediaInfo, out var origin)) {
                    var dateTime = mediaInfo.createTime;
                    var extension = Path.GetExtension(file);
                    var mediaType = mediaInfo.isImage ? "重复照片" : "重复照片";
                    var targetPath = $"{target}/{mediaType}/{dateTime.ToString(PathFormat)}/{mediaInfo.md5}_{mediaInfo.size}/";
                    var targetFile = GetFileName(targetPath, dateTime, extension);
                    FileUtil.CopyFile(file, targetFile, true);
                    if (!FileUtil.FileExist($"{targetPath}/info.txt")) {
                        FileUtil.CreateFile($"{targetPath}/info.txt", origin);
                    }
                    if (!FileUtil.FileExist($"{targetPath}/origin{extension}")) {
                        FileUtil.CopyFile(origin, $"{targetPath}/origin{extension}", true);
                    }
                } else {
                    var dateTime = mediaInfo.createTime;
                    var mediaType = mediaInfo.isImage ? "照片" : "视频";
                    var targetFile = GetFileName($"{target}/{mediaType}/{dateTime.ToString(PathFormat)}/", dateTime, Path.GetExtension(file));
                    distinctFiles[mediaInfo] = targetFile;
                    FileUtil.CopyFile(file, targetFile, true);
                }
            }
        }
        public static void SortMusic(string source, string target, bool clear) {
            var albums = new Dictionary<string, Album>();
            var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
            var total = files.Length;
            if (clear) FileUtil.DeleteFolder(target);
            var progress = new Progress(total);
            for (var i = 0; i < total; ++i) {
                progress.SetProgress(i);
                var file = files[i];
                var musicInfo = Util.GetMusicInfo(file);
                if (musicInfo == null || musicInfo is TagLib.Jpeg.File) {
                    continue;
                }
                var performers = new HashSet<string>();
                Array.ForEach(musicInfo.Tag.Performers, x => performers.UnionWith(x.Split("&")));
                var singer = string.Join("&", performers);
                var albumName = musicInfo.Tag.Album;
                var targetPath = $"{target}/{singer}/{albumName}/";
                var targetFile = $"{targetPath}{singer}-{musicInfo.Tag.Title}{Path.GetExtension(file)}";
                FileUtil.CopyFile(file, targetFile, true);
                if (string.IsNullOrEmpty(albumName)) continue;
                if (!albums.TryGetValue(albumName, out var album)) {
                    album = albums[albumName] = new Album();
                }
                var data = album.GetData(singer);
                if (data == null) {
                    data = new Album.Data() { singer = singer, performers = performers, path = targetPath };
                    album.datas.Add(data);
                }
                data.files.Add(targetFile);
            }
            void SortAlbum(Album album) {
                while (album.datas.Count > 1) {
                    album.datas.Sort((a, b) => b.files.Count.CompareTo(a.files.Count));
                    var first = album.datas[0];
                    album.datas.RemoveAt(0);
                    for (var i = 0; i < album.datas.Count; ) {
                        var data = album.datas[i];
                        if (first.performers.Overlaps(data.performers)) {
                            foreach (var file in data.files) {
                                var targetFile = $"{first.path}/{Path.GetFileName(file)}";
                                logger.info($"移动文件 : {file} -> {targetFile}");
                                FileUtil.MoveFile(file, targetFile, true);
                            }
                            album.datas.RemoveAt(i);
                        } else {
                            ++i;
                        }
                    }
                }
            }
            foreach (var pair in albums) {
                SortAlbum(pair.Value);
            }
            FileUtil.DeleteEmptyFolder(target, true);
        }
        //public static void Distinct(string source, string backup) {
        //    var hashFiles = new Dictionary<MediaInfo, DistinctFile>();
        //    var files = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        //    var total = files.Length;
        //    var progress = new Progress(total);
        //    for (var i = 0; i < total; i++) {
        //        progress.SetProgress(i);
        //        var file = files[i];
        //        var mediaInfo = Util.GetMediaInfo(file);
        //        if (mediaInfo == null) {
        //            logger.info($"{file} 文件不是有效的媒体文件");
        //            continue;
        //        }
        //        if (!hashFiles.TryGetValue(mediaInfo, out var distinctFile)) {
        //            hashFiles.Add(mediaInfo, new DistinctFile() { file = file });
        //            continue;
        //        }
        //        if (!FileUtil.CompareFile(distinctFile.file, file)) {
        //            continue;
        //        }
        //        var pathName = (mediaInfo.isImage ? "图片/" : "视频/") + Path.GetFileNameWithoutExtension(distinctFile.file);
        //        var extension = Path.GetExtension(distinctFile.file);
        //        if (!distinctFile.isBackup) {
        //            distinctFile.isBackup = true;
        //            FileUtil.CopyFile(distinctFile.file, $"{backup}/{pathName}/{Guid.NewGuid()}{extension}");
        //        }
        //        FileUtil.MoveFile(file, $"{backup}/{pathName}/{Guid.NewGuid()}{extension}", true);
        //        logger.info($"发现重复文件 {file} -> {distinctFile.file}");
        //    }
        //}
    }
}