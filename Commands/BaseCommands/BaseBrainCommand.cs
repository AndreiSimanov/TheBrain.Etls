using Microsoft.Extensions.Configuration;
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

    public override void GetUsage() { }

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
