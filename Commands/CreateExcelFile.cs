using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands;

class CreateExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    public override string GetCommandName()
    {
        return AppResources.CreateExcelFile;
    }

    protected async override Task RunCommandAsync()
    {
        await base.RunCommandAsync();
        await CreateResultFileAsync();
    }

    async Task CreateResultFileAsync()
    {
        if (thoughts.Count == 0)
        {
            errors.Add(string.Format(AppResources.FilesNotFound, config[Consts.CONTENT_FILE_NAME]));
            return;
        }

        EtlLog.Information(AppResources.AddContentToExcel);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(Consts.SHEET_NAME);
        var rowIndex = 0;
        foreach (var thought in thoughts)
        {
            ++rowIndex;
            worksheet.Cells[$"{Consts.ID_COL}{rowIndex}"].Value = thought.Value.Id;
            worksheet.Cells[$"{Consts.NAME_COL}{rowIndex}"].Value = thought.Value.Name;
            if (!string.IsNullOrWhiteSpace(thought.Value.ContentPath))
                worksheet.Cells[$"{Consts.CONTENT_COL}{rowIndex}"].Value = await File.ReadAllTextAsync(thought.Value.ContentPath);
            EtlLog.Processed(rowIndex, thoughts.Count);
        }

        var excelFilePath = config[Consts.EXCEL_FILE_PATH];

        EtlLog.Information(string.Format(AppResources.SavingExcelFile, excelFilePath));
        if (File.Exists(excelFilePath))
            File.Delete(excelFilePath);
        await package.SaveAsAsync(excelFilePath);
    }
}