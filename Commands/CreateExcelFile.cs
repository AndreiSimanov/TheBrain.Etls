using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;

namespace TheBrain.Etls.Commands;

internal class CreateExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    protected override void RunCommand()
    {
        base.RunCommand();

        var maxId = markedFiles.Keys.Max();

        foreach (var newFile in newFiles)
        {
            AddFileId(newFile, ++maxId);
            Console.WriteLine($"New file {maxId} = {newFile}");
        }

        CreateResultFile();
    }

    void CreateResultFile()  //todo: CreateResultFileAsync
    {
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
        }
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
