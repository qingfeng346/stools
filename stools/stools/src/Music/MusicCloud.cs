using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

public class MusicCloud : MusicBase {
    public class Songs {
        public List<MusicInfo> songs;
    }
    public class MusicInfo {
        public string name;
        public List<MusicArtist> artists;
        public MusicAlbum album;
    }
    public class MusicArtist {
        public string name;
        public string picUrl;
    }
    public class MusicAlbum {
        public string name;
        public string picUrl;
    }
    //歌曲下载地址 : http://music.163.com/song/media/outer/url?id=ID数字.mp3
    protected override async Task ParseInfo(string id) {
        var songs = JsonConvert.DeserializeObject<Songs>(await HttpUtil.Get($"http://music.163.com/api/song/detail/?ids=[{id}]"));
        var musicInfo = songs.songs[0];
        Name = musicInfo.name;
        Album = musicInfo.album.name;
        CoverUrls.Add(musicInfo.album.picUrl);
        foreach (var artist in musicInfo.artists) {
            Singer.Add(artist.name);
            CoverUrls.Add(artist.picUrl);
        }
        Mp3Urls.Add($"http://music.163.com/song/media/outer/url?id={id}.mp3");
    }
}
