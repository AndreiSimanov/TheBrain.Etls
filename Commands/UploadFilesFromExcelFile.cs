using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Serilog;
using TheBrain.Etls.Commands.BaseCommands;

namespace TheBrain.Etls.Commands;

internal class UploadFilesFromExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    protected override void RunCommand()
    {
        base.RunCommand();
        UploadFiles();
    }

    void UploadFiles()  //todo: UploadFilesAsync
    {
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];

        var fileInfo = new FileInfo(excelFilePath!);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        using FileStream stream = fileInfo.OpenRead();
        package.Load(stream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Rows.Count();
        if (rowCount == 0)
        {
            errors.Add($"Excel file '{excelFilePath}' is empty.");
            return;
        }

        for (int rowIndex = 1; rowIndex <= rowCount; rowIndex++)
        {
            var id = worksheet.Cells[$"A{rowIndex}"].Value.ToString();

            if (ValidateId(rowIndex, id))
            {
                var contentPath = GetFilePath(id!);
                if (File.Exists(contentPath))
                    File.WriteAllText(contentPath, worksheet.Cells[$"C{rowIndex}"].Value.ToString()); //todo: WriteAllTextAsync
            }
            WriteProgress(rowIndex, rowCount);
        }
        Console.WriteLine(string.Empty);
    }

    protected override void ValidateParams()
    {
        base.ValidateParams();
        if (!File.Exists(config[Consts.EXCEL_FILE_PATH]))
            errors.Add($"Excel file '{config[Consts.EXCEL_FILE_PATH]}' not found.");
    }

    bool ValidateId(int rowIndex, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Log.Warning($"Row {rowIndex} doesn't contain Id");
            return false;
        }

        if (!thoughts.ContainsKey(id))
        {
            Log.Warning($"Row {rowIndex}: thought '{id}' doesn't exist in db.");
            return false;
        }

        var contentPath = GetFilePath(id!);
        if (string.IsNullOrWhiteSpace(contentPath))
        {
            Log.Warning($"Row {rowIndex}: file '{contentPath}' doesn't exist.");
            return false;
        }
        return true;
    }
}
