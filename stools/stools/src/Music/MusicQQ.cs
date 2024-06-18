using System.Threading.Tasks;
using Newtonsoft.Json;
using HtmlAgilityPack;
using Scorpio.stools;
public class MusicQQ : MusicBase {
    public override string Source => MusicFactory.QQ;
    public class QQDownloadInfo {
        public class Data {
            public string name;
            public string singer;
            public string album;
            public string cover;
            public string url;
            public string lrc;
        }
        public int code;
        public Data data;
    }
    protected override async Task ParseInfo(string id) {
        var downloadInfo = await $"https://api.leafone.cn/api/qqmusic?id={id}".Get<QQDownloadInfo>();
        Name = downloadInfo.data.name;
        Album = downloadInfo.data.album;
        Lyrics = downloadInfo.data.lrc;
        Singer.Add(downloadInfo.data.singer);
        CoverUrls.Add(downloadInfo.data.cover);
        Mp3Urls.Add(downloadInfo.data.url);
    }
    protected override async Task<AlbumInfo> ParseAlbum_impl(string id) {
        var albumInfo = new AlbumInfo();
        return albumInfo;
    }
}
