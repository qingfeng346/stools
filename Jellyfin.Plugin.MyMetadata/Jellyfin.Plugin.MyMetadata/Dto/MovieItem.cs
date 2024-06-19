using MediaBrowser.Controller.Entities;
namespace Jellyfin.Plugin.MyMetadata.Dto
{
    /// <summary> 影片详情 </summary>
    public class MovieItem {
        /// <summary> 识别码 </summary>
        public string Id { get; set; }
        /// <summary> 识别码 </summary>
        public string ServiceId { get; set; }
        /// <summary> 标题 </summary>
        public string? Title { get; set; }
        /// <summary> 发行日期 </summary>
        public DateTime? ReleaseDate { get; set; }
        /// <summary> 时长 </summary>
        public int Duration { get; set; }
        /// <summary> 简介 </summary>
        public string Intro { get; set; }
        /// <summary> 大封面 </summary>
        public string Fanart { get; set; }
        /// <summary> 小封面 </summary>
        public string Poster { get; set; }
        /// <summary> 来源URL </summary>
        public string SourceUrl { get; set; }
        /// <summary> 导演 </summary>
        public List<PersonInfo> Directors { get; private set; } = new List<PersonInfo>();
        /// <summary> 工作室 </summary>
        public List<string> Studios { get; private set; } = new List<string>();
        /// <summary> 发行商 </summary>
        public List<string> Labels { get; private set; } = new List<string>();
        /// <summary> 系列 </summary>
        public List<string> Series { get; private set; } = new List<string>();
        /// <summary> 类别 </summary>
        public List<string> Genres { get; private set; } = new List<string>();
        /// <summary> 演员 </summary>
        public List<PersonInfo> Persons { get; private set; } = new List<PersonInfo>();
        /// <summary> 预览图 </summary>
        public List<string> Shotscreens { get; private set; } = new List<string>();
    }
}
