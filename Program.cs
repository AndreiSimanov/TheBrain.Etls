// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;
using TheBrain.Etls.Commands;
using TheBrain.Etls.Commands.BaseCommands;
using TheBrain.Etls;

AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

IConfiguration config = new ConfigurationBuilder().AddCommandLine(args).Build();
InitConfig(config);

Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(config[Consts.LOG_FILE_PATH]!)
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Error)
            .CreateLogger();

var commands = new List<BaseCommand> {
    new CreateExcelFile(config),
    new UploadFilesFromExcelFile(config),
};

try
{
    if (commands.Any(command => command.Run()))
        return;

    //var opt = new Options();
    //Console.WriteLine(opt.GetUsage());
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    Console.WriteLine(ex.StackTrace);
}

Console.ReadKey();


void InitConfig(IConfiguration config)
{
    if (string.IsNullOrWhiteSpace(config[Consts.DB_FILE_NAME]))
        config[Consts.DB_FILE_NAME] = Consts.DEFAULT_DB_FILE_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.CONTENT_FILE_NAME]))
        config[Consts.CONTENT_FILE_NAME] = Consts.DEFAULT_CONTENT_FILE_NAME;

    if (string.IsNullOrWhiteSpace(config[Consts.LOG_FILE_PATH]))
        config[Consts.LOG_FILE_PATH] = Consts.DEFAULT_LOG_FILE_NAME;
}

void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) => Log.Error(e.ExceptionObject.ToString()!);
