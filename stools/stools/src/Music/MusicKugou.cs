using System.Threading.Tasks;
using Newtonsoft.Json;
using HtmlAgilityPack;
public class MusicKugou : MusicBase {
    public override string Source => MusicFactory.Kugou;
    public class MusicUrlInfo {
        public string hash;
    }
    public class MusicInfo {
        public int err_code;
        public MusicData data;
    }
    public class MusicData {
        public string hash;
        public string song_name;
        public string author_name;
        public string album_id;
        public string album_name;
        public string img;
        public string play_url;
        public string play_backup_url;
    }
    protected override async Task ParseInfo(string id) {
        var musicInfo = JsonConvert.DeserializeObject<MusicInfo>(await HttpUtil.Get($"http://www.kugou.com/yy/index.php?r=play/getdata&hash={id}", (message) => {
            message.Headers.Add("Cookie", "kg_mid=42e7c301308736085f5b226d29ef20c51111");
        }));
        var result = await HttpUtil.Get($"http://www.kugou.com/yy/index.php?r=play/getdata&hash={id}&album_id={musicInfo.data.album_id}", (message) => {
            message.Headers.Add("Cookie", "kg_mid=42e7c301308736085f5b226d29ef20c51111");
        });
        musicInfo = JsonConvert.DeserializeObject<MusicInfo>(result);
        Name = musicInfo.data.song_name;
        Album = musicInfo.data.album_name;
        Singer.Add(musicInfo.data.author_name);
        CoverUrls.Add(musicInfo.data.img);
        Mp3Urls.Add(musicInfo.data.play_url);
        Mp3Urls.Add(musicInfo.data.play_backup_url);
    }
    protected override async Task<AlbumInfo> ParseAlbum_impl(string id) {
        await Task.Delay(1);
        var html = new HtmlWeb();
        var doc = html.Load($"https://www.kugou.com/album/info/{id}/").DocumentNode;
        var details = doc.SelectSingleNode("//p[@class='detail']").InnerText.Split('\n');
        var albumInfo = new AlbumInfo();
        foreach (var deltail in details) {
            if (deltail.Contains("专辑名")) {
                var index = deltail.LastIndexOf("：");
                albumInfo.name = deltail.Substring(index + 1).Replace("<br />", "").Trim();
            } else if (deltail.Contains("歌手")) {
                var index = deltail.LastIndexOf("：");
                albumInfo.artist = deltail.Substring(index + 1).Replace("<br />", "").Trim();
            }
        }
        var nodes = doc.SelectNodes("//ul[@class='songList']/li/a");
        foreach (var node in nodes) {
            var data = node.GetAttributeValue("data", "");
            var hash = data.Substring(0, data.LastIndexOf("|"));
            albumInfo.musicList.Add(hash);
        }
        return albumInfo;
    }
}
