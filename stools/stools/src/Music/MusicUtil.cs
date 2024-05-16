using Scorpio.Commons;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using System;
using System.IO;
using SixLabors.ImageSharp.Processing;
using TagLib;

namespace Scorpio.stools {
    public class MusicInfo {
        public string Name;
        public string[] Singer;
        public string Album;
        public uint? Year;
        public uint? Track;
        public string Publisher;
        public string Lyrics;
        public string[] Genres;
        public string Picture;
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
    public class MusicUtil {
        public static void SetMusicInfo(string mp3File, MusicInfo musicInfo) {
            var file = TagLib.File.Create(mp3File);
            file.Tag.Title = musicInfo.Name ?? file.Tag.Title;
            file.Tag.Performers = musicInfo.Singer ?? file.Tag.Performers;
            file.Tag.Album = musicInfo.Album ?? file.Tag.Album;
            file.Tag.Year = musicInfo.Year ?? file.Tag.Year;
            file.Tag.Track = musicInfo.Track ?? file.Tag.Track;
            file.Tag.Publisher = musicInfo.Publisher ?? file.Tag.Publisher;
            file.Tag.Lyrics = musicInfo.Lyrics ?? file.Tag.Lyrics;
            file.Tag.Genres = musicInfo.Genres ?? file.Tag.Genres;
            if (!string.IsNullOrWhiteSpace(musicInfo.Picture) && FileUtil.FileExist(musicInfo.Picture)) {
                using (Image<Rgba32> image = Image.Load<Rgba32>(musicInfo.Picture)) {
                    image.Mutate(x => x.Resize(512, 512));
                    image.Save(musicInfo.Picture);
                }
                var pictures = new List<IPicture>();
                {
                    var picture = new TagLib.Id3v2.AttachmentFrame(new Picture(musicInfo.Picture));
                    picture.Type = PictureType.FrontCover;
                    picture.Filename = "frontcover.png";
                    picture.Description = "";
                    picture.MimeType = Picture.GetMimeFromExtension(musicInfo.Picture);
                    picture.TextEncoding = StringType.Latin1;
                    pictures.Add(picture);
                }
                file.Tag.Pictures = pictures.ToArray();
            }
            file.Save();
        }
        public static void SortMusic(string source, string target, bool clear, bool move) {
            var albums = new Dictionary<string, Album>();
            void AddMusic(TagLib.Tag tag, string file) {
                var albumName = tag.Album;
                var performers = new HashSet<string>();
                Array.ForEach(tag.Performers, x => performers.UnionWith(x.Split("&")));
                var singer = string.Join("&", performers);
                var targetFile = $"{target}/{singer}/{albumName}/{singer}-{tag.Title}{Path.GetExtension(file)}";
                if (Path.GetFullPath(file) != Path.GetFullPath(targetFile)) {
                    FileUtil.MoveFile(file, targetFile, true);
                }
                if (string.IsNullOrEmpty(albumName)) return;
                if (!albums.TryGetValue(albumName, out var album)) {
                    album = albums[albumName] = new Album();
                }
                var data = album.GetData(singer);
                if (data == null) {
                    data = new Album.Data() { singer = singer, performers = performers, path = Path.GetDirectoryName(targetFile) };
                    album.datas.Add(data);
                }
                data.files.Add(targetFile);
            }
            bool MoveFile(string sourceFile, string targetFile, bool forceMove) {
                if (!System.IO.File.Exists(sourceFile))
                    return false;
                var m = move || forceMove;
                if (System.IO.File.Exists(targetFile)) {
                    if (new FileInfo(sourceFile).Length > new FileInfo(targetFile).Length) {
                        if (m)
                            FileUtil.MoveFile(sourceFile, targetFile, true);
                        else
                            FileUtil.CopyFile(sourceFile, targetFile, true);
                    } else if (m) {
                        FileUtil.DeleteFile(sourceFile);
                    }
                    return true;
                } else {
                    if (m)
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
            if (FileUtil.PathExist(source) && Path.GetFullPath(source) != Path.GetFullPath(target)) {
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
                    if (MoveFile(sourceFile, targetFile, false))
                        repeatCount++;
                    AddMusic(musicInfo.Tag, targetFile);
                }
            }
            void SortAlbum(Album album) {
                while (album.datas.Count > 1) {
                    album.datas.Sort((a, b) => b.files.Count.CompareTo(a.files.Count));
                    var first = album.datas[0];
                    album.datas.RemoveAt(0);
                    for (var i = 0; i < album.datas.Count; ++i) {
                        var data = album.datas[i];
                        if (first.performers.Overlaps(data.performers)) {
                            foreach (var file in data.files) {
                                var targetFile = $"{first.path}/{Path.GetFileName(file)}";
                                logger.info($"移动文件 {file} -> {targetFile}");
                                if (MoveFile(file, targetFile, true)) {
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
                    SortAlbum(pair.Value);
                }
            }
            logger.info("清理空目录");
            FileUtil.DeleteEmptyFolder(source, true);
            FileUtil.DeleteEmptyFolder(target, true);
            logger.info($"整理完成,所有文件:{fileCount},无效文件:{invalidCount},重复文件:{repeatCount}");
        }
    }
}
