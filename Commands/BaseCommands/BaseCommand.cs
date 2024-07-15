using Microsoft.Extensions.Configuration;
using Serilog;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseCommand(IConfiguration config)
{
    protected readonly IConfiguration config = config;
    protected readonly List<string> errors = new List<string>();

    public bool Run()
    {
        if (config[Consts.COMMAND] == null)
            return false;

        if (string.Equals(GetType().Name.ToLower(), config[Consts.COMMAND]!.ToLower()))
        {
            EtlLog.Information($"Run {GetType().Name}");

            ValidateParams();
            if (errors.Count > 0)
            {
                errors.ForEach(error => EtlLog.Information(error));
                return false;
            }

            RunCommand();
            if (errors.Count > 0)
            {
                errors.ForEach(error => EtlLog.Information(error));
                return false;
            }

            EtlLog.Information($"{GetType().Name} command completed.");
            return true;
        }
        return false;
    }

    protected abstract void RunCommand();
    protected abstract void ValidateParams();
    public abstract void GetUsage();
}
