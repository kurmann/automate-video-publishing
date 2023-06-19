using Microsoft.Extensions.Configuration;
using NLog;

class Program
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var options = new Options();
        configuration.Bind(options);

        // Workflow-Kontext erstellen
        var contextResult = WorkflowContext.Create(options.QuickTimeMasterDirectory, options.PublishedMpeg4Directory)
            .Tap(context => Logger.Info($"Executing workflow with quick time masterfile directory: {context.QuickTimeMasterDirectory.Directory}"))
            .Tap(context => Logger.Info($"Executing workflow with published MPEG-4 directory: {context.PublishedMpeg4Directory.Directory}"));
        if (contextResult.IsFailure)
        {
            Logger.Info($"Error setting up workflow context: {contextResult.Error}");
            return;
        }

        // Strategie mappen und ausführen
        var workflowResult = WorkflowStrategyMapper.Create(options.Workflow)
            .Map(strategyMapper => strategyMapper.SelectedStrategyHandler.Execute(contextResult.Value));
        Logger.Info(workflowResult.IsFailure ? workflowResult.Error : "Workflow completed");

    }
}


public class Options
{
    public string? QuickTimeMasterDirectory { get; set; }

    public string? PublishedMpeg4Directory { get; set; }

    public string? Workflow { get; set; }
}
