using Microsoft.Extensions.Configuration;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseCommand(IConfiguration config)
{
    protected readonly IConfiguration config = config;
    protected readonly List<string> errors = new List<string>();

    public async Task<bool> RunAsync()
    {
        if (config[Consts.COMMAND] == null)
            return false;

        if (string.Equals(GetType().Name.ToLower(), config[Consts.COMMAND]!.ToLower()))
        {
            EtlLog.Information(string.Format(AppResources.RunCommand, GetCommandName()));

            ValidateParams();
            if (errors.Count > 0)
            {
                errors.ForEach(error => EtlLog.Error(error));
                EtlLog.Information(AppResources.СommandCompletedError);
                return true;
            }

            await RunCommandAsync();

            if (errors.Count > 0)
            {
                errors.ForEach(error => EtlLog.Error(error));
                EtlLog.Information(AppResources.СommandCompletedError);
                return true;
            }

            EtlLog.Information(AppResources.СommandCompleted);
            return true;
        }
        return false;
    }

    protected abstract Task RunCommandAsync();
    protected abstract void ValidateParams();
    public abstract void GetUsage();
    public abstract string GetCommandName();
}
