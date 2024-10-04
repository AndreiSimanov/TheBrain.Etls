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

    protected override void ValidateParams()
    {
        base.ValidateParams();
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (!string.IsNullOrWhiteSpace(excelFilePath) && !File.Exists(excelFilePath))
            errors.Add(string.Format(AppResources.ExcelFileNotFound, excelFilePath));
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
            var contentPath = GetFilePath(id!);
            var content = worksheet.Cells[$"{Consts.CONTENT_COL}{rowIndex}"].Value?.ToString();
            if (ValidateRow(rowIndex, id, contentPath, content))
            {
                await UpdateContent(contentPath, content);
            }
            EtlLog.Processed(rowIndex, rowCount);
        }
    }

    async Task UpdateContent(string contentPath, string? content)
    {
        if (File.Exists(contentPath))
        {
            var attrs = File.GetAttributes(contentPath);
            var roFlag = attrs.HasFlag(FileAttributes.ReadOnly);
            if (roFlag)
                File.SetAttributes(contentPath, attrs & ~FileAttributes.ReadOnly);

            await File.WriteAllTextAsync(contentPath, content);

            if (roFlag)
                File.SetAttributes(contentPath, attrs & FileAttributes.ReadOnly);
        }
    }

    bool ValidateRow(int rowIndex, string? id, string contentPath, string? content)
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

        if (!File.Exists(contentPath) && !string.IsNullOrWhiteSpace(content))
        {
            EtlLog.Warning(string.Format(AppResources.RowFileNotFound, rowIndex, contentPath));
            return false;
        }
        return true;
    }
}
