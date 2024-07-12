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

        for (int rowIndex = 1; rowIndex <= rowCount; rowIndex++)
        {
            var id = worksheet.Cells[$"A{rowIndex}"].Value.ToString();

            if (!string.IsNullOrWhiteSpace(id))
            {
                var contentPath = GetFilePath(id);
                if (!string.IsNullOrWhiteSpace(contentPath))
                {
                    File.WriteAllText(contentPath, worksheet.Cells[$"C{rowIndex}"].Value.ToString()); //todo: WriteAllTextAsync
                }
                else
                {
                    //todo: log "file not found"
                }
            }
            WriteProgress(rowIndex, rowCount);
        }
        Console.WriteLine(string.Empty);
    }
}
