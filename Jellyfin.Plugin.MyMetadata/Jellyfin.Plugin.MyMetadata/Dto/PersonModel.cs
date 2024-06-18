using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.MyMetadata.Dto
{
    /// <summary>
    /// 人员信息
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Alt { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public PersonAvatar Avatars { get; set; } = new PersonAvatar { };
    }
}
