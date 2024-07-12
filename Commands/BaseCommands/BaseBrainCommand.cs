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
        Console.WriteLine("RunCommand");
        var dirPath = config[Consts.BRAINS_FOLDER_PATH];

        if (string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath))
        {
            Console.WriteLine($"Path {dirPath} not found.");
            return;
        }

        var excelFilePath = config[Consts.EXCEL_FILE_PATH];

        if (string.IsNullOrWhiteSpace(dirPath) || !Directory.Exists(dirPath))
        {
            Console.WriteLine($"Excel file path {excelFilePath} not found.");
            return;
        }

        var files = Directory.EnumerateFiles(dirPath, "*.md", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            Console.WriteLine($"Found file: {file}");

            var text = File.ReadLines(file).Where(txt => !string.IsNullOrWhiteSpace(txt)).FirstOrDefault();

            var id = GetId(text);
            if (id.HasValue)
            {
                markedFiles.Add(id.Value, file);
                continue;
            }

            newFiles.Add(file);
        }

        foreach (var markedFile in markedFiles)
            Console.WriteLine($"Marked file {markedFile.Key} = {markedFile.Value}");
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
}
