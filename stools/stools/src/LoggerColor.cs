using System;

public class LoggerColor : IDisposable {
    private ConsoleColor color;
    public LoggerColor(ConsoleColor color) {
        this.color = Console.ForegroundColor;
        Console.ForegroundColor = color;
    }
    public void Dispose() {
        Console.ForegroundColor = color;
    }
}
