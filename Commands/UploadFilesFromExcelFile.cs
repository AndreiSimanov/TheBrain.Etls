using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;
using TheBrain.Etls.Resources.Languages;

namespace TheBrain.Etls.Commands;

internal class UploadFilesFromExcelFile(IConfiguration config) : BaseBrainCommand(config)
{
    public override string GetCommandName()
    {
        return AppResources.UploadFilesFromExcelFile;
    }

    protected async override Task RunCommandAsync()
    {
        await base.RunCommandAsync();
        await UploadFilesAsync();
    }

    async Task UploadFilesAsync()
    {
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];

        var fileInfo = new FileInfo(excelFilePath!);
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage();
        using FileStream stream = fileInfo.OpenRead();
        await package.LoadAsync(stream);
        var worksheet = package.Workbook.Worksheets[0];
        var rowCount = worksheet.Rows.Count();
        if (rowCount == 0)
        {
            errors.Add(string.Format(AppResources.ExcelFileEmpty, excelFilePath));
            return;
        }

        for (int rowIndex = 1; rowIndex <= rowCount; rowIndex++)
        {
            var id = worksheet.Cells[$"{Consts.ID_COL}{rowIndex}"].Value?.ToString();

            if (ValidateId(rowIndex, id))
            {
                var contentPath = GetFilePath(id!);
                await File.WriteAllTextAsync(contentPath, worksheet.Cells[$"{Consts.CONTENT_COL}{rowIndex}"].Value.ToString());
            }
            EtlLog.Processed(rowIndex, rowCount);
        }
    }

    protected override void ValidateParams()
    {
        base.ValidateParams();
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (!string.IsNullOrWhiteSpace(excelFilePath) && !File.Exists(excelFilePath))
            errors.Add(string.Format(AppResources.ExcelFileNotFound, excelFilePath));
    }

    bool ValidateId(int rowIndex, string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            EtlLog.Warning(string.Format(AppResources.RowWithoutId, rowIndex));
            return false;
        }

        if (!thoughts.ContainsKey(id))
        {
            EtlLog.Warning(string.Format(AppResources.RowWrongId, rowIndex, id));
            return false;
        }

        var contentPath = GetFilePath(id!);
        if (string.IsNullOrWhiteSpace(contentPath))
        {
            EtlLog.Warning(string.Format(AppResources.RowFileNotFound, rowIndex, contentPath));
            return false;
        }
        return true;
    }
}
