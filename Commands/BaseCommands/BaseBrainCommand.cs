using Microsoft.Extensions.Configuration;
using System.Text;
using TheBrain.Etls.DBContext;
using TheBrain.Etls.Models;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseBrainCommand(IConfiguration config) : BaseCommand(config)
{
    protected Dictionary<string, Thought> thoughts = new();
    protected int filesCount = 0;
    protected string brainsFolderPath = string.Empty;
    protected string contentFileName = string.Empty;

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

    protected override void RunCommand()
    {
        brainsFolderPath = config[Consts.BRAINS_FOLDER_PATH]!;
        contentFileName = config[Consts.CONTENT_FILE_NAME]!;
        var dbFile = Path.Combine(brainsFolderPath!, config[Consts.DB_FILE_NAME]!);

        EtlLog.Information(string.Format(AppResources.LoadDataFromDb, Path.Combine(brainsFolderPath!, config[Consts.DB_FILE_NAME]!)));

        thoughts = GetThoughts(dbFile);

        EtlLog.Information(string.Format(AppResources.FindFiles, config[Consts.CONTENT_FILE_NAME], brainsFolderPath));

        foreach (var thought in thoughts)
        {
            var contentPath = GetFilePath(thought.Key);
            if (File.Exists(contentPath))
            {
                thought.Value.ContentPath = contentPath;
                filesCount++;
            }
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
        }
    }

    protected string GetFilePath(string id) => Path.Combine(brainsFolderPath, id, contentFileName);

    Dictionary<string, Thought> GetThoughts(string dbFile)
    {
        using var dbContext = new SqliteContext(dbFile);
        return dbContext.Thoughts.ToDictionary(item => item.Id);
    }
}
