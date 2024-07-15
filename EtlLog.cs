using Serilog;
using Serilog.Events;

namespace TheBrain.Etls;

public static class EtlLog
{
    static bool isProgress = false;

    public static void Init(string logPath)
    {
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(logPath)
                    .CreateLogger();
    }

    public static void Information(string message, bool isWriteToConsole = true)
    {
        if (isWriteToConsole)
        {
            ResetIsProgress();
            Console.WriteLine(message);
        }
        Log.Information(message);
    }

    public static void Warning(string message, bool isWriteToConsole = false)
    {
        if (isWriteToConsole)
            ConsoleWriteLine(message, ConsoleColor.Yellow);
        Log.Warning(message);
    }

    public static void Error(string message, bool isWriteToConsole = true)
    {
        if (isWriteToConsole)
            ConsoleWriteLine(message, ConsoleColor.Red);
        Log.Error(message);
    }

    public static void Progress(int current, int total)
    {
        //Lock for multythreding Interlocked.
        isProgress = true;
        Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        Console.Write($"Progress: {current} of {total}");
    }

    public static void ResetIsProgress()
    {
        if (isProgress)  //Lock for multythreding Interlocked.
        {
            isProgress = false;
            Console.WriteLine(string.Empty);
        }
    }

    static void ConsoleWriteLine(string message, ConsoleColor color)
    {
        ResetIsProgress();
        var saveColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = saveColor;
    }
}
