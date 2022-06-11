using System.Collections.Generic;
using Scorpio.Commons;
public static class Extension {
    public static string GetMemory(this long length) {
        return ScorpioUtil.GetMemory(length);
    }
    public static string GetSingers(this List<string> value) {
        return string.Join("&", value);
    }
}
