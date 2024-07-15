using Serilog;
using System.Text;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls;

public static class EtlLog
{
    static bool isProcessed = false;

    public static void Init(string logPath)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.File(logPath)
                    .CreateLogger();
    }


    public static void Information(string message, bool isWriteToConsole = true)
    {
        if (isWriteToConsole)
            ConsoleWriteLine(message);
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

    public static void Processed(int current, int total)
    {
        //Lock for multythreding Interlocked.
        isProcessed = true;
        Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        Console.Write(string.Format(AppResources.Processed, current, total));
    }

    public static void ResetIsProgress()
    {
        if (isProcessed)  //Lock for multythreding Interlocked.
        {
            isProcessed = false;
            Console.WriteLine(string.Empty);
        }
    }

    public static void ConsoleWriteLine(string message, ConsoleColor? color = null)
    {
        var saveColor = Console.ForegroundColor;
        if (color != null)
            Console.ForegroundColor = color.Value;

        ResetIsProgress();
        Console.WriteLine(message);
        if (color != null)
            Console.ForegroundColor = saveColor;
    }
}
