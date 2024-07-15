using Microsoft.Extensions.Configuration;
using TheBrain.Etls.Models;
using TheBrain.Etls.DBContext;

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

        EtlLog.Information($"Load data from '{Path.Combine(brainsFolderPath!, config[Consts.DB_FILE_NAME]!)}'");
        thoughts = GetThoughts(dbFile);
        EtlLog.Information($"Scan '{config[Consts.CONTENT_FILE_NAME]}' files in '{brainsFolderPath}' folder.");

        foreach (var thought in thoughts)
        {
            var contentPath = GetFilePath(thought.Key);
            if (File.Exists(contentPath))
            {
                thought.Value.ContentPath = contentPath;
                filesCount++;
            }
        }

        EtlLog.Information($"Files count: {filesCount}");
    }

    public override void GetUsage() { }

    protected override void ValidateParams()
    {
        var brainsFolderPath = config[Consts.BRAINS_FOLDER_PATH];
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];

        if (string.IsNullOrWhiteSpace(brainsFolderPath) || !Directory.Exists(brainsFolderPath))
            errors.Add($"Path '{brainsFolderPath}' not found.");

        var dbFile = Path.Combine(brainsFolderPath!, config[Consts.DB_FILE_NAME]!);
        if (!File.Exists(dbFile))
            errors.Add($"Database file '{dbFile}' not found.");

        var outputPath = Path.GetDirectoryName(excelFilePath);

        if (string.IsNullOrWhiteSpace(outputPath) || !Directory.Exists(outputPath))
            errors.Add($"Excel file path '{outputPath}' not found.");

        var outputFileName = Path.GetFileName(excelFilePath);
        if (string.IsNullOrWhiteSpace(outputFileName))
            errors.Add($"Excel file name is empty.");
    }

    protected string GetFilePath(string id) => Path.Combine(brainsFolderPath, id, contentFileName);

    Dictionary<string, Thought> GetThoughts(string dbFile)
    {
        using var dbContext = new SqliteContext(dbFile);
        return dbContext.Thoughts.ToDictionary(item => item.Id);
    }
}
