using System.Text;
using TheBrain.Etls.Commands;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.CommandUsages;

class UploadFilesFromExcelFileUsage : BaseBrainCommandUsage
{
    protected override void GetOptionalUsage(StringBuilder sb)
    {
        base.GetOptionalUsage(sb);
        sb.AppendLine(string.Format(AppResources.UpdateAllThoughtsParamUsage, Consts.PARAM_USAGE_INDENT, Consts.UPDATE_ALL_THOUGHTS, Consts.DEFAULT_UPDATE_ALL_THOUGHTS));
    }

    protected override void GetUsageExamples(StringBuilder sb)
    {
        base.GetUsageExamples(sb);
        sb.AppendLine(string.Format(AppResources.CommandSample3,
            AppDomain.CurrentDomain.FriendlyName,
            Consts.COMMAND,
            GetCommandName(),
            Consts.EXCEL_FILE_PATH,
            Consts.BRAINS_FOLDER_PATH,
            Consts.UPDATE_ALL_THOUGHTS));
        sb.AppendLine(string.Empty);
    }

    protected override string GetCommandName()
    {
        return nameof(UploadFilesFromExcelFile);
    }

    protected override string GetCommandDescription()
    {
        return AppResources.UploadFilesFromExcelFile;
    }
}