using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;

namespace TheBrain.Etls.Commands;

internal class UploadFilesFromExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    protected override void RunCommand()
    {
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (!File.Exists(excelFilePath))
            throw new Exception($"Excel file '{excelFilePath}' not found.");
        base.RunCommand();
        UploadFiles();
    }


    void UploadFiles()  //todo: UploadFilesAsync
    {
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (!File.Exists(excelFilePath))
            return;

        var fileInfo = new FileInfo(excelFilePath);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        using FileStream stream = fileInfo.OpenRead();
        package.Load(stream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Rows.Count();
        if (rowCount == 0)
            throw new Exception($"Excel file '{excelFilePath}' is empty.");

        foreach (var row in worksheet.Rows)
        {
            UploadFile(row.Range.Text);
            WriteProgress(row.StartRow, rowCount);
        }
        Console.WriteLine(string.Empty);
    }

    void UploadFile(string text)//todo: UploadFileAsync
    {
        using var reader = new StringReader(text);
        var firstLine = reader.ReadLine();
        var id = GetId(firstLine);
        if (!id.HasValue)
        {
            Console.WriteLine($"Row {0} doesn't contain Id");
            return;
        }

        if (!markedFiles.ContainsKey(id.Value))
        {
            Console.WriteLine($"File with Id = {id.Value} doesn't exist");
            return;
        }

        File.WriteAllText(markedFiles[id.Value], text); //todo: WriteAllTextAsync
    }
}
