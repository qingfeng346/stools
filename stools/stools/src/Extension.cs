using System.Collections.Generic;
public static class Extension {
    public static string GetMemory(this long length) {
        if (length < 1024) {
            return length + "B";
        } else if (length < 1024 * 1024) {
            return (length / 1024) + "KB";
        } else if (length < 1024 * 1024 * 1024) {
            return (length / 1024 / 1024) + "MB";
        } else {
            return (length / 1024 / 1024 / 1024) + "GB";
        }
    }
    public static string GetSingers(this List<string> value) {
        return string.Join("&", value);
    }
}
