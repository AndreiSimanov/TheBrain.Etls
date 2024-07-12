using Microsoft.Extensions.Configuration;
using TheBrain.Etls.Models;
using TheBrain.Etls.DBContext;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseBrainCommand(IConfiguration config) : BaseCommand(config)
{
    protected Dictionary<string, Thoughts> thoughts = new();
    protected int filesCount = 0;
    protected  string brainsFolderPath = string.Empty;
    protected override void RunCommand()
    {
        brainsFolderPath = config[Consts.BRAINS_FOLDER_PATH];
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        CheckCommandParams(brainsFolderPath, excelFilePath);

        Console.WriteLine($"Load data from Brain.db in '{brainsFolderPath}' folder.");
        thoughts = GetThoughts(brainsFolderPath!);
        Console.WriteLine($"Scan files in '{brainsFolderPath}' folder.");

        foreach (var thing in thoughts)
        {
            var contentPath = GetFilePath(thing.Key);
            if (!string.IsNullOrWhiteSpace(contentPath))
            {
                thing.Value.ContentPath = contentPath;
                filesCount++;
            }
        }

        Console.WriteLine($"Files count: {filesCount}");
    }

    void CheckCommandParams(string? brainsFolderPath, string? excelFilePath)
    {
        if (string.IsNullOrWhiteSpace(brainsFolderPath) || !Directory.Exists(brainsFolderPath))
            throw new Exception($"Path '{brainsFolderPath}' not found.");

        var outputPath = Path.GetDirectoryName(excelFilePath);

        if (string.IsNullOrWhiteSpace(outputPath) || !Directory.Exists(outputPath))
            throw new Exception($"Excel file path '{outputPath}' not found.");

        var outputFileName = Path.GetFileName(excelFilePath);
        if (string.IsNullOrWhiteSpace(outputFileName))
            throw new Exception($"Excel file name is empty.");

    }

    protected string GetFilePath(string id)
    {
        var contentPath = Path.Combine(brainsFolderPath!, id, "Notes.md");
        return File.Exists(contentPath)? contentPath: string.Empty;
    }

    Dictionary<string, Thoughts> GetThoughts(string path)
    {
        var dbFile = Path.Combine(path, "Brain.db");
        if (!File.Exists(dbFile))
            throw new Exception($"Database file '{dbFile}' not found.");

        using var dbContext = new SqliteContext(dbFile);

        return dbContext.Thoughts.ToDictionary(item => item.Id);
    }
}
