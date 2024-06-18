﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.MyMetadata.Dto
{
    /// <summary>
    /// 搜索结果返回
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// 影片在网站的唯一编号
        /// </summary>
        public string Id { get; set; }
    }
}
