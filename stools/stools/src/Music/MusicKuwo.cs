using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
public class MusicKuwo : MusicBase {
    public override string Source => "酷我";
    public class MusicInfo {
        public int code;
        public MusicData data;
    }
    public class MusicData {
        public string name;
        public string artist;
        public string album;
        public string pic;
        public string pic120;
        public string releaseDate;
        public int track;
    }
    public class AlbumInfo {
        public int code;
        public AlbumData data;
    }
    public class AlbumData {
        public string artist;   //歌手
        public string releaseDate;
        public string album;
        public int albumid;
        public string pic;
        public string albuminfo;
        public string lang;
        public List<AlbumMusic> musicList;
    }
    public class AlbumMusic {
        public int rid;
        public string name;
        public int track;
    }
    protected override async Task ParseInfo(string id) {
        var musicInfo = JsonConvert.DeserializeObject<MusicInfo>(await HttpUtil.Get($"https://wapi.kuwo.cn/api/www/music/musicInfo?mid={id}"));
        var downloadUrl = await HttpUtil.Get($"http://antiserver.kuwo.cn/anti.s?type=convert_url&rid={id}&format=mp3|aac&response=url");
        Name = musicInfo.data.name;
        Album = musicInfo.data.album;
        Singer.Add(musicInfo.data.artist);
        CoverUrls.Add(musicInfo.data.pic);
        Mp3Urls.Add(downloadUrl);
        Track = (uint)musicInfo.data.track;
        if (!string.IsNullOrEmpty(musicInfo.data.releaseDate)) {
            var index = musicInfo.data.releaseDate.IndexOf("-");
            if (index >= 0 && int.TryParse(musicInfo.data.releaseDate.Substring(0, index), out var year)) {
                Year = (uint)year;
            }
        }
    }
    public override async Task<List<string>> ParseAlbum(string id) {
        var albumInfo = JsonConvert.DeserializeObject<AlbumInfo>(await HttpUtil.Get($"https://wapi.kuwo.cn/api/www/album/albumInfo?albumId={id}"));
        return albumInfo.data.musicList.ConvertAll(_ => _.rid.ToString());
    }
}
