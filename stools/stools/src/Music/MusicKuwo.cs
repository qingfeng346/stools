using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
public class MusicKuwo : MusicBase {
    public override string Source => "酷我";
    public class KuwoMusicInfo {
        public class Data {
            public string name;
            public string artist;
            public string album;
            public string pic;
            public string pic120;
            public string releaseDate;
            public int track;
        }
        public int code;
        public Data data;
    }
    public class KuwoAlbumInfo {
        public class Music {
            public int rid;
            public string name;
            public int track;
        }
        public class Data {
            public string artist;   //歌手
            public string releaseDate;
            public string album;
            public int albumid;
            public string pic;
            public string albuminfo;
            public string lang;
            public List<Music> musicList;
        }
        public int code;
        public Data data;
    }
    
    protected override async Task ParseInfo(string id) {
        var result = await HttpUtil.Get($"https://wapi.kuwo.cn/api/www/music/musicInfo?mid={id}");
        var musicInfo = JsonConvert.DeserializeObject<KuwoMusicInfo>(result);
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
    protected override async Task<AlbumInfo> ParseAlbum_impl(string id) {
        var albumInfo = JsonConvert.DeserializeObject<KuwoAlbumInfo>(await HttpUtil.Get($"https://wapi.kuwo.cn/api/www/album/albumInfo?albumId={id}"));
        return new AlbumInfo() { name = albumInfo.data.album, musicList = albumInfo.data.musicList.ConvertAll(_ => _.rid.ToString()) };
    }
}
