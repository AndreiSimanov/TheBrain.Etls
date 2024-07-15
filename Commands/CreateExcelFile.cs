using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands;

internal class CreateExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    public override string GetCommandName()
    {
        return AppResources.CreateExcelFile;
    }

    protected override void RunCommand()
    {
        base.RunCommand();
        CreateResultFile();
    }

    void CreateResultFile()  //todo: CreateResultFileAsync
    {
        if (filesCount == 0)
        {
            errors.Add(string.Format(AppResources.FilesNotFound, config[Consts.CONTENT_FILE_NAME]));
            return;
        }

        EtlLog.Information(AppResources.AddContentToExcel);

        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (File.Exists(excelFilePath))
            File.Delete(excelFilePath);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(Consts.SHEET_NAME);
        var rowIndex = 0;
        foreach (var thought in thoughts)
        {
            if (string.IsNullOrWhiteSpace(thought.Value.ContentPath))
                continue;
            ++rowIndex;
            worksheet.Cells[$"{Consts.ID_COL}{rowIndex}"].Value = thought.Value.Id;
            worksheet.Cells[$"{Consts.NAME_COL}{rowIndex}"].Value = thought.Value.Name;
            worksheet.Cells[$"{Consts.CONTENT_COL}{rowIndex}"].Value = File.ReadAllText(thought.Value.ContentPath);
            EtlLog.Processed(rowIndex, filesCount);
        }

        EtlLog.Information(string.Format(AppResources.SavingExcelFile, excelFilePath));
        package.SaveAs(excelFilePath); //todo: SaveAsAsync
    }
}