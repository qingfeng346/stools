using Jellyfin.Data.Enums;
using Jellyfin.Plugin.MyMetadata.Service;
using MediaBrowser.Controller.Entities;
using WMJson;
namespace Jellyfin.Plugin.MyMetadata.Dto {
    /// <summary> 演员详情 </summary>
    public class PersonItem {
        /// <summary> 名字 </summary>
        public string Name { get; set; }
        /// <summary> 封面 </summary>
        public string ImageUrl { get; set; }
        public string PersonId {get;set;}
        public PersonInfo ToPersonInfo() {
            return new PersonInfo() {
                Name = Name,
                Role = Name,
                Type = PersonKind.Actor,
                ItemId = Utils.GetGuidByName(Name),
                ImageUrl = ImageUrl
            };
        } 
    }
}
