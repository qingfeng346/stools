using System;
using Scorpio.Commons;
public class ConsoleLogger : ILogger {
    private string Now => DateTime.Now.ToString(TimeUtil.TimeFormat);
    public void debug(string value) {
        Console.WriteLine($"[{Now}][debug] {value}");
    }
    public void info(string value) {
        Console.WriteLine($"[{Now}] {value}");
    }
    public void warn(string value) {
        Console.WriteLine($"[{Now}][warn] {value}");
    }
    public void error(string value) {
        Console.Error.WriteLine($"[{Now}][error] {value}");
    }
}
