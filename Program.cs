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

Console.ReadKey();


void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
{
    Console.WriteLine(e.ExceptionObject.ToString());
}
