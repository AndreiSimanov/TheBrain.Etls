using Microsoft.Extensions.Configuration;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands.BaseCommands;

abstract class BaseCommand(IConfiguration config)
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

    protected virtual void ValidateParams()
    {
        EtlLog.Information(AppResources.CommandParams, false);
        foreach (var param in config.AsEnumerable())
            EtlLog.Information($"{param.Key} = {param.Value}", false);
    }

    protected abstract Task RunCommandAsync();
    public abstract string GetCommandName();
}
