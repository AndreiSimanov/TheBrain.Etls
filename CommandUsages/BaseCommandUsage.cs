using System.Text;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.CommandUsages;

abstract class BaseCommandUsage
{
    public string GetUsage()
    {
        var sb = new StringBuilder();
        GetMandatoryUsage(sb);
        GetOptionalUsage(sb);
        sb.AppendLine(string.Empty);
        GetUsageExamples(sb);
        return sb.ToString();
    }

    protected virtual void GetMandatoryUsage(StringBuilder sb)
    {
        sb.AppendLine(GetCommandDescription());
        sb.AppendLine(AppResources.CommandUsage);
    }

    protected virtual void GetOptionalUsage(StringBuilder sb) {
        sb.AppendLine(AppResources.CommandOptionalParams);
    }

    protected virtual void GetUsageExamples(StringBuilder sb) => sb.AppendLine(AppResources.CommandExamples);

    protected abstract string GetCommandName();
    protected abstract string GetCommandDescription();
}