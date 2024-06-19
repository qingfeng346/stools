using System.Security.Cryptography;
using System.Text;

namespace Jellyfin.Plugin.MyMetadata.Service {
    public static class Utils {
        public static Guid GetGuidByName(string name) {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(name);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                return new Guid(hashBytes);
            }
        }
    }
}