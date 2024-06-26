﻿using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System;
using Scorpio.stools;
public class MusicKuwo : MusicBase {
    public override string Source => MusicFactory.Kuwo;
    public class KuwoMusicInfo {
        public class Data {
            public string name;
            public string artist;
            public int artistid;
            public string album;
            public int albumid;
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
            public string artist;           //歌手
            public string releaseDate;      //发行时间
            public string album;            //专辑名字
            public int albumid;             //专辑ID
            public string pic;              //专辑封面
            public string albuminfo;        //专辑介绍
            public string lang;             //语言
            public List<Music> musicList;   //歌曲列表
        }
        public int code;
        public Data data;
    }
    public class KuwoLyric {
        public class Lrc {
            public string lineLyric;
            public string time;
        }
        public class Data {
            public List<Lrc> lrclist;
        }
        public Data data;
        public int status;
    }
    public class KuwoDownloadInfo {
        public class Data {
            public string name;
            public string singer;
            public string album;
            public string cover;
            public string url;
        }
        public int code;
        public Data data;
    }
    protected override async Task ParseInfo(string id) {
        var musicInfo = await $"https://wapi.kuwo.cn/api/www/music/musicInfo?mid={id}".Get<KuwoMusicInfo>();
        // var downloadUrl = await HttpUtil.Get($"http://antiserver.kuwo.cn/anti.s?type=convert_url&rid={id}&format=mp3|aac&response=url");
        //var downloadUrl = await HttpUtil.Get($"https://service-4v0argn6-1314197819.gz.apigw.tencentcs.com/rid/?rid={id}");
        var downloadInfo = await $"https://api.leafone.cn/api/kuwo?id={id}".Get<KuwoDownloadInfo>();
        var downloadUrl = downloadInfo.data.url;
        Name = musicInfo.data.name;
        Album = musicInfo.data.album;
        Singer.AddRange(musicInfo.data.artist.Split("&"));
        CoverUrls.Add(musicInfo.data.pic);
        Mp3Urls.Add(downloadUrl);
        Track = (uint)musicInfo.data.track;
        if (!string.IsNullOrEmpty(musicInfo.data.releaseDate)) {
            var index = musicInfo.data.releaseDate.IndexOf("-");
            if (index >= 0 && int.TryParse(musicInfo.data.releaseDate.Substring(0, index), out var year)) {
                Year = (uint)year;
            }
        }
        var lyric = await $"http://m.kuwo.cn/newh5/singles/songinfoandlrc?musicId={id}".Get<KuwoLyric>();
        if (lyric?.data?.lrclist != null) {
            var builder = new StringBuilder();
            foreach (var lyc in lyric.data.lrclist) {
                var time = float.Parse(lyc.time);
                var min = Math.Floor(time / 60);
                var sec = time % 60;
                builder.AppendLine(string.Format("[{0:00}:{1:00.00}]{2}", min, sec, lyc.lineLyric));
            }
            Lyrics = builder.ToString();
        }
    }
    protected override async Task<AlbumInfo> ParseAlbum_impl(string id) {
        var albumInfo = await $"https://wapi.kuwo.cn/api/www/album/albumInfo?albumId={id}".Get<KuwoAlbumInfo>();
        return new AlbumInfo() { 
            name = albumInfo.data.album,
            artist = albumInfo.data.artist,
            musicList = albumInfo.data.musicList.ConvertAll(_ => _.rid.ToString()) };
    }
}
