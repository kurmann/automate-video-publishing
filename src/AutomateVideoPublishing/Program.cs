using AutomateVideoPublishing.Strategies;
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

        // Gewählte Strategie instanzieren
        var strategyResult = WorkflowStrategyMapper.Create(options.Workflow);
        if (strategyResult.IsFailure)
        {
            Logger.Error($"Error on selecting strategy: {strategyResult.Error}");
            return;
        }
        
        // Erstellen Sie den Observer und abonnieren Sie ihn bei der Strategy
        var logObserver = new LogObserver();
        strategyResult.Value.SelectedStrategy.EventBroadcaster.Subscribe(logObserver);

                // Workflow-Kontext erstellen
        var contextResult = WorkflowContext.Create(options.QuickTimeMasterDirectory, options.PublishedMpeg4Directory)
            .Tap(context => Logger.Info($"Executing workflow with quick time masterfile directory: {context.QuickTimeMasterDirectory.Directory}"))
            .Tap(context => Logger.Info($"Executing workflow with published MPEG-4 directory: {context.PublishedMpeg4Directory.Directory}"));
        if (contextResult.IsFailure)
        {
            Logger.Error($"Error createing worfklow context: {contextResult.Error}");
            return;
        }

        // Ausführung der Strategie
        var executionResult = strategyResult.Value.SelectedStrategy.Execute(contextResult.Value);

        // Falls Sie den Observer hier beenden möchten
        logObserver.OnCompleted();
    }
}


public class Options
{
    public string? QuickTimeMasterDirectory { get; set; }

    public string? PublishedMpeg4Directory { get; set; }

    public string? Workflow { get; set; }
}
