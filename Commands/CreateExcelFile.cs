using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;

namespace TheBrain.Etls.Commands;

internal class CreateExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    protected override void RunCommand()
    {
        base.RunCommand();
        CreateResultFile();
    }

    void CreateResultFile()  //todo: CreateResultFileAsync
    {
        if (filesCount == 0)
        {
            errors.Add($"Content '{config[Consts.CONTENT_FILE_NAME]}' files not found.");
            return;
        }

        Console.WriteLine("Adding files to excel file...");

        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (File.Exists(excelFilePath))
            File.Delete(excelFilePath); 

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("TheBrain");
        var rowIndex = 0;
        foreach (var thought in thoughts)
        {
            if (string.IsNullOrWhiteSpace(thought.Value.ContentPath))
                continue;
            ++rowIndex;
            worksheet.Cells[$"A{rowIndex}"].Value = thought.Value.Id;
            worksheet.Cells[$"B{rowIndex}"].Value = thought.Value.Name;
            worksheet.Cells[$"C{rowIndex}"].Value = File.ReadAllText(thought.Value.ContentPath);
            WriteProgress(rowIndex, filesCount);
        }
        Console.WriteLine(string.Empty);
        Console.WriteLine($"Saving excel file to '{excelFilePath}'...");
        package.SaveAs(excelFilePath); //todo: SaveAsAsync
    }
}