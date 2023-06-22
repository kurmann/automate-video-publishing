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
        
        // Abonniere die Statusnachrichten von der gewählten Strategie
        var logObserver = new LogObserver();
        var strategyUnsubscriber = strategyResult.Value.SelectedStrategy.EventBroadcaster.Subscribe(logObserver);

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

        // Diese Methode wird aufgerufen, um dem LogObserver zu signalisieren, dass das Observable, auf das er
        // abonniert hat (in diesem Fall der EventBroadcaster der gewählten Strategie), keine weiteren Daten 
        // senden wird. Nachdem diese Methode aufgerufen wurde, sollte der LogObserver keine weiteren 
        // OnNext- oder OnError-Aufrufe von diesem Observable erwarten. 
        logObserver.OnCompleted();

        // Diese Methode wird aufgerufen, um den LogObserver effektiv von der Liste der Observer des 
        // Observables zu entfernen. Durch Aufrufen dieser Methode teilen wir dem Observable mit, dass es keine
        // weiteren Daten an den LogObserver senden soll. Diese Methode ist besonders nützlich, wenn das Observable
        // weiter Daten senden könnte, der spezifische Observer (in diesem Fall der LogObserver) jedoch keine
        // weiteren Daten empfangen möchte. In diesem speziellen Fall, in dem das Programm kurz danach endet, 
        // ist es möglicherweise nicht notwendig, diese Methode aufzurufen, jedoch sorgt dies für eine saubere
        // und korrekte Verwendung des Observer-Patterns.
        strategyUnsubscriber.Dispose();
    }
}


public class Options
{
    public string? QuickTimeMasterDirectory { get; set; }

    public string? PublishedMpeg4Directory { get; set; }

    public string? Workflow { get; set; }
}
