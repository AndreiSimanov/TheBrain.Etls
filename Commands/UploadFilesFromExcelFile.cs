using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using TheBrain.Etls.Commands.BaseCommands;
using TheBrain.Etls.DBContext;
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
        await UploadDataAsync();
    }

    protected override void ValidateParams()
    {
        base.ValidateParams();
        var excelFilePath = config[Consts.EXCEL_FILE_PATH];
        if (!string.IsNullOrWhiteSpace(excelFilePath) && !File.Exists(excelFilePath))
            errors.Add(string.Format(AppResources.ExcelFileNotFound, excelFilePath));
    }

    async Task UploadDataAsync()
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

        await UpdateDataAsync(worksheet, rowCount);
    }

    async Task UpdateDataAsync(ExcelWorksheet worksheet, int rowCount)
    {
        using var dbContext = new SqliteContext(dbFile);

        for (int rowIndex = 1; rowIndex <= rowCount; rowIndex++)
        {
            var thoughtId = worksheet.Cells[$"{Consts.ID_COL}{rowIndex}"].Value?.ToString();
            var thoughtName = worksheet.Cells[$"{Consts.NAME_COL }{rowIndex}"].Value?.ToString();
            var contentPath = GetFilePath(thoughtId!);
            var content = worksheet.Cells[$"{Consts.CONTENT_COL}{rowIndex}"].Value?.ToString();
            if (ValidateRow(rowIndex, thoughtId))
            {
                thoughtName = thoughtName ?? string.Empty;
                if (!string.Equals(thoughts[thoughtId!].Name, thoughtName))
                    await UpdateThoughtName(dbContext, thoughtId!, thoughtName);

                if (ValidateContentFile(rowIndex, contentPath, content))
                    await UpdateThoughtContent(contentPath, content);
            }
            EtlLog.Processed(rowIndex, rowCount);
        }
        dbContext.SaveChanges();
    }

    async Task UpdateThoughtName(SqliteContext dbContext, string thoughtId, string thoughtName)
    {
        var thought =  await dbContext.Thoughts.FindAsync(thoughtId);
        if (thought != null)
            thought.Name = thoughtName;
    }

    async Task UpdateThoughtContent(string contentPath, string? content)
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

    bool ValidateRow(int rowIndex, string? thoughtId)
    {
        if (string.IsNullOrWhiteSpace(thoughtId))
        {
            EtlLog.Warning(string.Format(AppResources.RowWithoutId, rowIndex));
            return false;
        }

        if (!thoughts.ContainsKey(thoughtId))
        {
            EtlLog.Warning(string.Format(AppResources.RowWrongId, rowIndex, thoughtId));
            return false;
        }
        return true;
    }

    bool ValidateContentFile(int rowIndex, string contentPath, string? content)
    {
        if (!File.Exists(contentPath) && !string.IsNullOrWhiteSpace(content))
        {
            EtlLog.Warning(string.Format(AppResources.RowFileNotFound, rowIndex, contentPath));
            return false;
        }
        return true;
    }
}
