using Microsoft.Extensions.Configuration;
using System.Text;
using TheBrain.Etls.DBContext;
using TheBrain.Etls.Models;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseBrainCommand(IConfiguration config) : BaseCommand(config)
{
    protected Dictionary<string, Thought> thoughts = new();
    protected string brainsFolderPath = config[Consts.BRAINS_FOLDER_PATH]!;
    protected string contentFileName = config[Consts.CONTENT_FILE_NAME]!;
    protected string oldFormatContentFileName = config[Consts.OLD_FORMAT_CONTENT_FILE_NAME]!;
    protected string oldFormatContentFolderName = config[Consts.OLD_FORMAT_CONTENT_FOLDER_NAME]!;
    protected string dbFile = Path.Combine(config[Consts.BRAINS_FOLDER_PATH]!, config[Consts.DB_FILE_NAME]!);

    public override void GetUsage()
    {
        var sb = new StringBuilder();
        sb.AppendLine(GetCommandName());
        sb.AppendLine(AppResources.CommandUsage);
        sb.AppendLine(string.Format(AppResources.CommandParamUsage, Consts.PARAM_USAGE_INDENT, Consts.COMMAND, GetType().Name));
        sb.AppendLine(string.Format(AppResources.ExcelParamUsage, Consts.PARAM_USAGE_INDENT, Consts.EXCEL_FILE_PATH));
        sb.AppendLine(string.Format(AppResources.DatabaseParamUsage, Consts.PARAM_USAGE_INDENT, Consts.BRAINS_FOLDER_PATH));
        sb.AppendLine(AppResources.CommandOptionalParams);
        sb.AppendLine(string.Format(AppResources.DatabaseFileParamUsage, Consts.PARAM_USAGE_INDENT, Consts.DB_FILE_NAME, Consts.DEFAULT_DB_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.ContentFileParamUsage, Consts.PARAM_USAGE_INDENT, Consts.CONTENT_FILE_NAME, Consts.DEFAULT_CONTENT_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.OldFormatContentFileNameParamUsage, Consts.PARAM_USAGE_INDENT, Consts.OLD_FORMAT_CONTENT_FILE_NAME, Consts.DEFAULT_OLD_FORMAT_CONTENT_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.OldFormatContentFolderNameParamUsage, Consts.PARAM_USAGE_INDENT, Consts.OLD_FORMAT_CONTENT_FOLDER_NAME, Consts.DEFAULT_OLD_FORMAT_CONTENT_FOLDER_NAME));
        sb.AppendLine(string.Format(AppResources.LogFileParamUsage, Consts.PARAM_USAGE_INDENT, Consts.LOG_FILE_PATH, Consts.DEFAULT_LOG_FILE_NAME));
        sb.AppendLine(string.Format(AppResources.LangParamUsage, Consts.PARAM_USAGE_INDENT, Consts.LANG, Consts.DEFAULT_LANG));
        sb.AppendLine(string.Empty);
        sb.AppendLine(AppResources.CommandExamples);
        sb.AppendLine(string.Format(AppResources.CommandSample1,
            AppDomain.CurrentDomain.FriendlyName,
            Consts.COMMAND,
            GetType().Name,
            Consts.EXCEL_FILE_PATH,
            Consts.BRAINS_FOLDER_PATH));
        sb.AppendLine(string.Empty);
        sb.AppendLine(string.Format(AppResources.CommandSample2,
            AppDomain.CurrentDomain.FriendlyName,
            Consts.COMMAND,
            GetType().Name,
            Consts.EXCEL_FILE_PATH,
            Consts.BRAINS_FOLDER_PATH,
            Consts.LANG));
        sb.AppendLine(string.Empty);
        EtlLog.ConsoleWriteLine(sb.ToString());
    }

    protected async override Task RunCommandAsync()
    {
        EtlLog.Information(string.Format(AppResources.LoadDataFromDb, Path.Combine(brainsFolderPath!, config[Consts.DB_FILE_NAME]!)));
        EtlLog.Information(string.Format(AppResources.FindFiles, config[Consts.CONTENT_FILE_NAME], brainsFolderPath));
        EtlLog.Information(string.Format(AppResources.FindFiles, config[Consts.OLD_FORMAT_CONTENT_FILE_NAME], brainsFolderPath));

        using var dbContext = new SqliteContext(dbFile);

        EtlLog.Information(string.Format(AppResources.ThoughtsCount, dbContext.Thoughts.Count()));

        var filesCount = 0;

        await foreach (var thought in dbContext.Thoughts.AsAsyncEnumerable())
        {
            var contentPath = GetContentPath(thought.Id);
            if (!string.IsNullOrWhiteSpace(contentPath))
            {
                thought.ContentPath = contentPath;
                filesCount++;
            }
            thoughts.Add(thought.Id, thought);
        }

        EtlLog.Information(string.Format(AppResources.FilesCount, filesCount));
    }

    protected override void ValidateParams()
    {
        var brainsFolderPath = config[Consts.BRAINS_FOLDER_PATH];
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];

        if (string.IsNullOrWhiteSpace(brainsFolderPath))
            errors.Add(string.Format(AppResources.ParameterNotSpecified, Consts.BRAINS_FOLDER_PATH));
        else
        {
            if (!Directory.Exists(brainsFolderPath))
                errors.Add(string.Format(AppResources.PathNotFound, brainsFolderPath));

            var dbFile = Path.Combine(brainsFolderPath, config[Consts.DB_FILE_NAME]!);
            if (!File.Exists(dbFile))
                errors.Add(string.Format(AppResources.DbFileNotFound, dbFile));
        }

        if (string.IsNullOrWhiteSpace(excelFilePath))
            errors.Add(string.Format(AppResources.ParameterNotSpecified, Consts.EXCEL_FILE_PATH));
        else
        {
            var outputPath = Path.GetDirectoryName(excelFilePath);

            if (string.IsNullOrWhiteSpace(outputPath) || !Directory.Exists(outputPath))
                errors.Add(string.Format(AppResources.ExcelFilePathNotFound, outputPath));

            var excelFileName = Path.GetFileName(excelFilePath);
            if (string.IsNullOrWhiteSpace(excelFileName))
                errors.Add(AppResources.ExcelFileNameEmpty);
            else if (File.Exists(excelFilePath) && IsFileLocked(excelFilePath))
                errors.Add(string.Format(AppResources.FileLocked, excelFilePath));
        }
    }

    protected string GetContentPath(string id)
    {
        var contentPath = Path.Combine(brainsFolderPath, id, contentFileName);
        if (File.Exists(contentPath))
            return contentPath;

        contentPath = Path.Combine(brainsFolderPath, id, oldFormatContentFolderName, oldFormatContentFileName);
        if (File.Exists(contentPath))
            return contentPath;

        return string.Empty;
    }

    bool IsFileLocked(string filePath)
    {
        try
        {
            var file = new FileInfo(filePath);
            using FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            stream.Close();
        }
        catch (IOException)
        {
            return true;
        }
        return false;
    }
}
