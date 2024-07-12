using Microsoft.Extensions.Configuration;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseCommand(IConfiguration config)
{
    protected readonly IConfiguration config = config;

    public bool Run()
    {
        if (config[Consts.COMMAND] == null)
            return false;

        if (string.Equals(GetType().Name.ToLower(), config[Consts.COMMAND]!.ToLower()))
        {
            Console.WriteLine($"Run {GetType().Name}");
            RunCommand();
            Console.WriteLine($"{GetType().Name} command completed.");
            return true;
        }
        return false;
    }

    protected abstract void RunCommand();

    public static void WriteProgress(int current, int total)
    {
        Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
        Console.Write($"Progress: {current} of {total}");
    }
}
