using Scorpio.Commons;
using System;
using System.Collections.Generic;
using System.IO;

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
            if (clear) {
                FileUtil.DeleteFolder(target);
            } else if (FileUtil.PathExist($"{target}/整理文件")) {
                foreach (var file in Directory.GetFiles($"{target}/整理文件", "*", SearchOption.AllDirectories)) {
                    var mediaInfo = Util.GetMediaInfo(file);
                    if (mediaInfo != null) {
                        distinctFiles[mediaInfo] = file;
                    }
                }
            }
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
                    continue;
                }
                if (distinctFiles.TryGetValue(mediaInfo, out var origin)) {
                    var dateTime = mediaInfo.createTime;
                    var extension = Path.GetExtension(file);
                    var mediaType = mediaInfo.isImage ? "重复照片" : "重复视频";
                    var targetPath = $"{target}/重复文件/{mediaType}/{dateTime.ToString(PathFormat)}/{mediaInfo.md5}_{mediaInfo.size}/";
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
                    var targetFile = GetFileName($"{target}/整理文件/{mediaType}/{dateTime.ToString(PathFormat)}/", dateTime, Path.GetExtension(file));
                    distinctFiles[mediaInfo] = targetFile;
                    FileUtil.CopyFile(file, targetFile, true);
                }
            }
        }
        
        public static void SortMusic(string source, string target, bool clear, bool move) {
            var albums = new Dictionary<string, Album>();
            void AddMusic(TagLib.Tag tag, string file) {
                var albumName = tag.Album;
                var performers = new HashSet<string>();
                Array.ForEach(tag.Performers, x => performers.UnionWith(x.Split("&")));
                var singer = string.Join("&", performers);
                if (string.IsNullOrEmpty(albumName)) return;
                if (!albums.TryGetValue(albumName, out var album)) {
                    album = albums[albumName] = new Album();
                }
                var data = album.GetData(singer);
                if (data == null) {
                    data = new Album.Data() { singer = singer, performers = performers, path = Path.GetDirectoryName(file) };
                    album.datas.Add(data);
                }
                data.files.Add(file);
            }
            bool MoveFile(string sourceFile, string targetFile) {
                if (File.Exists(targetFile)) {
                    if (new FileInfo(sourceFile).Length > new FileInfo(targetFile).Length) {
                        if (move)
                            FileUtil.MoveFile(sourceFile, targetFile, true);
                        else
                            FileUtil.CopyFile(sourceFile, targetFile, true);
                    } else if (move) {
                        FileUtil.DeleteFile(sourceFile);
                    }
                    return true;
                } else {
                    if (move)
                        FileUtil.MoveFile(sourceFile, targetFile, true);
                    else
                        FileUtil.CopyFile(sourceFile, targetFile, true);
                    return false;
                }
            }
            if (clear) FileUtil.DeleteFolder(target);
            if (FileUtil.PathExist(target)) {
                var files = FileUtil.GetFiles(target, "*", SearchOption.AllDirectories);
                var progress = new Progress(files.Count, "读取原数据 : ");
                for (var i = 0; i < files.Count; ++i) {
                    progress.SetProgress(i);
                    var file = files[i];
                    var musicInfo = Util.GetMusicInfo(file);
                    if (musicInfo == null || musicInfo is TagLib.Jpeg.File) continue;
                    AddMusic(musicInfo.Tag, file);
                }
            }
            var invalidCount = 0;
            var repeatCount = 0;
            var fileCount = 0;
            {
                var files = FileUtil.GetFiles(source, "*", SearchOption.AllDirectories);
                fileCount = files.Count;
                var progress = new Progress(fileCount, "复制文件 : ");
                for (var i = 0; i < fileCount; ++i) {
                    progress.SetProgress(i);
                    var sourceFile = files[i];
                    var musicInfo = Util.GetMusicInfo(sourceFile);
                    if (musicInfo == null || musicInfo is TagLib.Jpeg.File) {
                        invalidCount++;
                        continue;
                    }
                    var albumName = musicInfo.Tag.Album;
                    var performers = new HashSet<string>();
                    Array.ForEach(musicInfo.Tag.Performers, x => performers.UnionWith(x.Split("&")));
                    var singer = string.Join("&", performers);
                    var targetFile = $"{target}/{singer}/{albumName}/{singer}-{musicInfo.Tag.Title}{Path.GetExtension(sourceFile)}";
                    if (MoveFile(sourceFile, targetFile))
                        repeatCount++;
                    AddMusic(musicInfo.Tag, targetFile);
                }
            }
            void SortAlbum(Album album) {
                while (album.datas.Count > 1) {
                    album.datas.Sort((a, b) => b.files.Count.CompareTo(a.files.Count));
                    var first = album.datas[0];
                    album.datas.RemoveAt(0);
                    logger.info($"first数量 : " + first.files.Count);
                    for (var i = 0; i < album.datas.Count; ++i ) {
                        var data = album.datas[i];
                        logger.info($"其他数量 : " + data.files.Count);
                        if (first.performers.Overlaps(data.performers)) {
                            foreach (var file in data.files) {
                                if (MoveFile(file, $"{first.path}/{Path.GetFileName(file)}")) {
                                    repeatCount++;
                                }
                            }
                        }
                    }
                }
            }
            {
                var progress = new Progress(albums.Count, "整理专辑 : ");
                var index = 0;
                foreach (var pair in albums) {
                    progress.SetProgress(index++);
                    logger.info($"整理专辑 : " + pair.Key);
                    SortAlbum(pair.Value);
                }
            }
            logger.info("清理空目录");
            FileUtil.DeleteEmptyFolder(target, true);
            logger.info($"整理完成,所有文件:{fileCount},无效文件:{invalidCount},重复文件:{repeatCount}");
        }
    }
}