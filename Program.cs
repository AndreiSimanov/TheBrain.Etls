using Microsoft.Extensions.Configuration;
using TheBrain.Etls.Commands;
using TheBrain.Etls.Commands.BaseCommands;

AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
IConfiguration config = new ConfigurationBuilder().AddCommandLine(args).Build();

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


//Console.WriteLine($"{Consts.COMMAND}: {config[Consts.COMMAND]}");
//Console.WriteLine($"{Consts.EXCEL_FILE_PATH}: {config[Consts.EXCEL_FILE_PATH]}");
//Console.WriteLine($"{Consts.BRAINS_FOLDER_PATH}: {config[Consts.BRAINS_FOLDER_PATH]}");



//Console.WriteLine($"test: {config["test"]}");

//foreach (var item in config.AsEnumerable())
//    Console.WriteLine(item.Key + " " + item.Value);

void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Console.WriteLine(e.ExceptionObject.ToString());
}
