using Microsoft.Extensions.Configuration;
using System.Globalization;
using TheBrain.Etls;
using TheBrain.Etls.Commands;
using TheBrain.Etls.Commands.BaseCommands;
using TheBrain.Etls.CommandUsages;
using TheBrain.Etls.Resources.Languages;

AppDomain.CurrentDomain.UnhandledException += UnhandledException;

IConfiguration config = new ConfigurationBuilder().AddCommandLine(args).Build();
InitConfig(config);

Thread.CurrentThread.CurrentUICulture = new CultureInfo(config[Consts.LANG]!);
Thread.CurrentThread.CurrentCulture = new CultureInfo(config[Consts.LANG]!);

EtlLog.Init(config[Consts.LOG_FILE_PATH]!);

var logFileInfo = new FileInfo(config[Consts.LOG_FILE_PATH]!);

var commands = new List<BaseCommand> {
    new CreateExcelFile(config),
    new UploadFilesFromExcelFile(config)};

var commandUsages = new List<BaseCommandUsage> {
    new CreateExcelFileUsage(),
    new UploadFilesFromExcelFileUsage()};

try
{
    foreach (var command in commands)
    {
        if (await command.RunAsync())
        {
            EtlLog.ConsoleWriteLine(string.Format(AppResources.SeeLogFilePath, logFileInfo.FullName));
            return;
        }
    }

    commandUsages.ForEach(commandUsage => EtlLog.ConsoleWriteLine(commandUsage.GetUsage()));
}
catch (Exception ex)
{
    EtlLog.Error(ex.Message);
    if (!string.IsNullOrWhiteSpace(ex.StackTrace))
        EtlLog.Error(ex.StackTrace);
    EtlLog.ConsoleWriteLine(string.Format(AppResources.SeeLogFilePath, logFileInfo.FullName));
}

Console.ReadKey();

void InitConfig(IConfiguration config)
{
    if (string.IsNullOrWhiteSpace(config[Consts.BRAINS_FOLDER_PATH]))
        config[Consts.BRAINS_FOLDER_PATH] = Consts.DEFAULT_BRAINS_FOLDER_PATH;

    if (string.IsNullOrWhiteSpace(config[Consts.DB_FILE_NAME]))
        config[Consts.DB_FILE_NAME] = Consts.DEFAULT_DB_FILE_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.CONTENT_FILE_NAME]))
        config[Consts.CONTENT_FILE_NAME] = Consts.DEFAULT_CONTENT_FILE_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.OLD_FORMAT_CONTENT_FILE_NAME]))
        config[Consts.OLD_FORMAT_CONTENT_FILE_NAME] = Consts.DEFAULT_OLD_FORMAT_CONTENT_FILE_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.OLD_FORMAT_CONTENT_FOLDER_NAME]))
        config[Consts.OLD_FORMAT_CONTENT_FOLDER_NAME] = Consts.DEFAULT_OLD_FORMAT_CONTENT_FOLDER_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.LOG_FILE_PATH]))
        config[Consts.LOG_FILE_PATH] = Consts.DEFAULT_LOG_FILE_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.LANG]))
        config[Consts.LANG] = Consts.DEFAULT_LANG;

    if (string.IsNullOrWhiteSpace(config[Consts.UPDATE_ALL_THOUGHTS]))
        config[Consts.UPDATE_ALL_THOUGHTS] = Consts.DEFAULT_UPDATE_ALL_THOUGHTS;
}

void UnhandledException(object sender, UnhandledExceptionEventArgs e) => EtlLog.Error(e.ExceptionObject.ToString()!);
