using System.Text;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.CommandUsages;

abstract class BaseBrainCommandUsage :BaseCommandUsage
{
    protected override void GetMandatoryUsage(StringBuilder sb)
    {
        base.GetMandatoryUsage(sb);
        sb.AppendLine(string.Format(AppResources.CommandParamUsage, Consts.PARAM_USAGE_INDENT, Consts.COMMAND, GetCommandName()));
        sb.AppendLine(string.Format(AppResources.ExcelParamUsage, Consts.PARAM_USAGE_INDENT, Consts.EXCEL_FILE_PATH));
        sb.AppendLine(string.Format(AppResources.DatabaseParamUsage, Consts.PARAM_USAGE_INDENT, Consts.BRAINS_FOLDER_PATH));
    }

    protected override void GetOptionalUsage(StringBuilder sb)
    {
        base.GetOptionalUsage(sb);
        sb.AppendLine(string.Format(AppResources.DatabaseFileParamUsage, Consts.PARAM_USAGE_INDENT, Consts.DB_FILE_NAME, Consts.DEFAULT_DB_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.ContentFileParamUsage, Consts.PARAM_USAGE_INDENT, Consts.CONTENT_FILE_NAME, Consts.DEFAULT_CONTENT_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.OldFormatContentFileNameParamUsage, Consts.PARAM_USAGE_INDENT, Consts.OLD_FORMAT_CONTENT_FILE_NAME, Consts.DEFAULT_OLD_FORMAT_CONTENT_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.OldFormatContentFolderNameParamUsage, Consts.PARAM_USAGE_INDENT, Consts.OLD_FORMAT_CONTENT_FOLDER_NAME, Consts.DEFAULT_OLD_FORMAT_CONTENT_FOLDER_NAME));
        sb.AppendLine(string.Format(AppResources.LogFileParamUsage, Consts.PARAM_USAGE_INDENT, Consts.LOG_FILE_PATH, Consts.DEFAULT_LOG_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.LangParamUsage, Consts.PARAM_USAGE_INDENT, Consts.LANG, Consts.DEFAULT_LANG));
    }

    protected override void GetUsageExamples(StringBuilder sb)
    {
        base.GetUsageExamples(sb);
        sb.AppendLine(string.Format(AppResources.CommandSample1,
            AppDomain.CurrentDomain.FriendlyName,
            Consts.COMMAND,
            GetCommandName(),
            Consts.EXCEL_FILE_PATH,
            Consts.BRAINS_FOLDER_PATH));
        sb.AppendLine(string.Empty);
        sb.AppendLine(string.Format(AppResources.CommandSample2,
            AppDomain.CurrentDomain.FriendlyName,
            Consts.COMMAND,
            GetCommandName(),
            Consts.EXCEL_FILE_PATH,
            Consts.BRAINS_FOLDER_PATH,
            Consts.LANG));
        sb.AppendLine(string.Empty);
    }
}