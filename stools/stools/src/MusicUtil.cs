using Scorpio.Commons;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using TagLib;
using SixLabors.ImageSharp.Processing;

public class MusicInfo {
    public string Name;
    public List<string> Singer = new List<string>();
    public string Album;
    public uint Year;
    public uint Track;
    public string Publisher;
    public string Lyrics;
    public List<string> Genres = new List<string>();
    public string Picture;
}
public class MusicUtil {
    public static void SetMusicInfo(string mp3File, MusicInfo musicInfo) {
        var file = File.Create(mp3File);
        file.Tag.Title = musicInfo.Name;
        file.Tag.Performers = musicInfo.Singer.ToArray();
        file.Tag.Album = musicInfo.Album;
        file.Tag.Year = musicInfo.Year;
        file.Tag.Track = musicInfo.Track;
        file.Tag.Publisher = musicInfo.Publisher;
        file.Tag.Lyrics = musicInfo.Lyrics;
        file.Tag.Genres = musicInfo.Genres.ToArray();
        if (!string.IsNullOrWhiteSpace(musicInfo.Picture) && FileUtil.FileExist(musicInfo.Picture)) {
            using (Image<Rgba32> image = Image.Load<Rgba32>(musicInfo.Picture)) {
                image.Mutate(x => x.Resize(512, 512));
                image.Save(musicInfo.Picture);
            }
            var pictures = new List<IPicture>(); {
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
}
