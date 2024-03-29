﻿using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using RestSharp;
public class MusicCloud : MusicBase {
    public override string Source => MusicFactory.Cloud;
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
        public class Artist {
            public string name;     //演唱者
        }
        public class Album {
            public string name;         //专辑名字
            public long publishTime;    //发行时间
            public Artist artist;       //演唱者
            public List<Song> songs;    //歌曲列表
        }
        public int code;
        public string message;
        public Album album;
        public Artist artist;
    }
    public class CloudLyric {
        public class Lyric {
            public string version;
            public string lyric;
        }
        public int code;
        public Lyric lrc;
        public Lyric klyric;
        public Lyric tlyric;
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
        //var mp3url = await HttpUtil.Get($"http://v.api.aa1.cn/api/wymusic/index.php?id={id}");
        //Mp3Urls.Add(mp3url);
        var lyric = JsonConvert.DeserializeObject<CloudLyric>(await HttpUtil.Get($"http://music.163.com/api/song/lyric?os=pc&id={id}&lv=-1&kv=-1&tv=-1"));
        if (!string.IsNullOrWhiteSpace(lyric?.lrc?.lyric)) {
            Lyrics = lyric.lrc.lyric;
        } else if (!string.IsNullOrWhiteSpace(lyric?.klyric?.lyric)) {
            Lyrics = lyric.lrc.lyric;
        } else if (!string.IsNullOrWhiteSpace(lyric?.tlyric?.lyric)) {
            Lyrics = lyric.lrc.lyric;
        }
    }
    protected override async Task<AlbumInfo> ParseAlbum_impl(string id) {
        var client = new RestClient($"http://music.163.com");
        //var request = new RestRequest($"api/album/{id}?ext=true&id={id}&offset=0&total=true&limit=10");
        var request = new RestRequest($"api/album/{id}?ext=true&id={id}");
        var result = await client.GetAsync(request);
        var albumInfo = JsonConvert.DeserializeObject<CloudAlbumInfo>(result.Content);
        if (albumInfo?.code != 200) {
            throw new System.Exception($"{albumInfo?.code} : {albumInfo?.message}");
        }
        return new AlbumInfo() { 
            name = albumInfo.album.name,
            artist = albumInfo.artist?.name ?? albumInfo.album.artist?.name,
            musicList = albumInfo.album.songs.ConvertAll(_ => _.id.ToString()) };
    }
}
