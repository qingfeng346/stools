using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

public class MusicCloud : MusicBase {
    public override string Source => "网易云音乐";
    public class CloudMusicInfos {
        public class Song {
            public class Artist {
                public string name;
                public string picUrl;
            }
            public class Album {
                public string name;
                public string picUrl;
                public long publishTime;
            }
            public string name;
            public List<Artist> artists;
            public Album album;
            public int position;
        }
        public List<Song> songs;
    }
    public class CloudAlbumInfo {
        public class Song {
            public int id;
            public string name;
        }
        public class Album {
            public string name;
            public long publishTime;
            public List<Song> songs;
        }
        public int code;
        public Album album;
    }

    //歌曲下载地址 : http://music.163.com/song/media/outer/url?id=ID数字.mp3
    protected override async Task ParseInfo(string id) {
        var songs = JsonConvert.DeserializeObject<CloudMusicInfos>(await HttpUtil.Get($"http://music.163.com/api/song/detail/?ids=[{id}]"));
        var musicInfo = songs.songs[0];
        Name = musicInfo.name;
        Album = musicInfo.album.name;
        CoverUrls.Add(musicInfo.album.picUrl);
        Year = (uint)TimeUtil.GetDateTime(musicInfo.album.publishTime).Year;
        Track = (uint)musicInfo.position;
        foreach (var artist in musicInfo.artists) {
            Singer.Add(artist.name);
            CoverUrls.Add(artist.picUrl);
        }
        Mp3Urls.Add($"http://music.163.com/song/media/outer/url?id={id}.mp3");
    }
    protected override async Task<AlbumInfo> ParseAlbum_impl(string id) {
        var albumInfo = JsonConvert.DeserializeObject<CloudAlbumInfo>(await HttpUtil.Get($"http://music.163.com/api/album/{id}"));
        return new AlbumInfo() { name = albumInfo.album.name, musicList = albumInfo.album.songs.ConvertAll(_ => _.id.ToString()) };
    }
}
