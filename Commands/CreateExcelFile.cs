using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.Runtime.Intrinsics.X86;
using TheBrain.Etls.Commands.BaseCommands;

namespace TheBrain.Etls.Commands;

internal class CreateExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    protected override void RunCommand()
    {
        base.RunCommand();

        if (newFiles.Count > 0)
        {
            Console.WriteLine("Adding 'Id' to new files...");
            var maxId = markedFiles.Keys.Max();
            for (int i = 0; i < newFiles.Count; i++)
            {
                AddFileId(newFiles[i], ++maxId);
                markedFiles.Add(maxId, newFiles[i]);
                WriteProgress(i + 1, newFiles.Count);
            }
            Console.WriteLine(string.Empty);
        }
        CreateResultFile();
    }

    void CreateResultFile()  //todo: CreateResultFileAsync
    {
        if (markedFiles.Count == 0)
            throw new Exception("'*.md' files not found.");

        Console.WriteLine("Adding files to excel file...");

        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (File.Exists(excelFilePath))
            File.Delete(excelFilePath); 

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("TheBrain");
        var rowIndex = 0;
        foreach (var id in markedFiles.Keys.OrderBy(id => id))
        {
            var file = markedFiles[id];
            worksheet.Cells[$"A{++rowIndex}"].Value = File.ReadAllText(file);
            WriteProgress(rowIndex, markedFiles.Count);
        }
        Console.WriteLine(string.Empty);
        Console.WriteLine($"Saving excel file to '{excelFilePath}'...");
        package.SaveAs(excelFilePath); //todo: SaveAsAsync
    }


    static void AddFileId(string filename, int id) //todo: AddFileIdAsync
    {
        var tempfile = Path.GetTempFileName();
        using (var writer = new StreamWriter(tempfile))
        using (var reader = new StreamReader(filename))
        {
            writer.WriteLine($"Id:{id}"); //todo: WriteLineAsync
            while (!reader.EndOfStream)
                writer.WriteLine(reader.ReadLine());
        }
        File.Copy(tempfile, filename, true);
        File.Delete(tempfile);
    }
}
