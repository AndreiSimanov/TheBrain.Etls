using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;

namespace TheBrain.Etls.Commands.BaseCommands;

internal abstract class BaseBrainCommand(IConfiguration config) : BaseCommand(config)
{
    readonly Regex regex = new Regex(@"ID:\d+");
    protected Dictionary<int, string> markedFiles = new();
    protected List<string> collisionFiles = new();
    protected List<string> newFiles = new();

    protected override void RunCommand()
    {
        var brainsFolderPath = config[Consts.BRAINS_FOLDER_PATH];
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        CheckCommandParams(brainsFolderPath, excelFilePath);

        Console.WriteLine($"Scan '*.md' files in '{brainsFolderPath}' folder.");

        var files = Directory.EnumerateFiles(brainsFolderPath!, "*.md", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            var text = File.ReadLines(file).Where(txt => !string.IsNullOrWhiteSpace(txt)).FirstOrDefault();

            var id = GetId(text);
            if (id.HasValue)
            {
                if (markedFiles.ContainsKey(id.Value))
                    collisionFiles.Add(file);
                else
                    markedFiles.Add(id.Value, file);
                continue;
            }
            newFiles.Add(file);
        }

        Console.WriteLine($"New files count: {newFiles.Count}");
        Console.WriteLine($"Files with 'Id' count: {markedFiles.Count}");

        if (collisionFiles.Count > 0) 
            Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Collision files count: {collisionFiles.Count}");
        Console.ResetColor();
    }

    public int? GetId(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;
        text = text.Trim().Replace(" ", string.Empty).ToUpper();

        if (regex.IsMatch(text))
        {
            var idStr = text.Split(":")[1];
            if (int.TryParse(idStr, out int id))
                return id;
        }
        return null;
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
}
