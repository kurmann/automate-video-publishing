using AutomateVideoPublishing.Strategies;
using Microsoft.Extensions.Configuration;
using NLog;

class Program
{
    // Definiere einen Logger für die Anwendung. Die Konfiguration des Loggers erfolgt 
    // durch die NLog-Konfigurationsdatei (nlog.config).
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    // Die Main-Methode ist der Einstiegspunkt der Konsolenanwendung. 
    static void Main(string[] args)
    {
        // Erstelle ein Konfigurationsobjekt, das Umgebungsvariablen und Befehlszeilenargumente
        // berücksichtigt.
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Die Optionen aus der Konfiguration werden in ein Options-Objekt gebunden.
        var options = new Options();
        configuration.Bind(options);

        // Erzeuge eine Instanz der ausgewählten Workflow-Strategie mithilfe eines Strategie-Mappers.
        // Bei einem Fehler wird ein entsprechender Log-Eintrag erstellt und das Programm wird beendet.
        var strategyResult = WorkflowStrategyMapper.Create(options.Workflow);
        if (strategyResult.IsFailure)
        {
            Logger.Error($"Error on selecting strategy: {strategyResult.Error}");
            return;
        }
        
        // Erzeuge einen LogObserver und abonniere die Ereignisse der ausgewählten Workflow-Strategie.
        var logObserver = new LogObserver();
        var strategyUnsubscriber = strategyResult.Value.SelectedStrategy.EventBroadcaster.Subscribe(logObserver);

        // Erzeuge den Workflow-Kontext, der Daten enthält, die für alle Workflow-Strategien nützlich sind. 
        // Bei einem Fehler wird ein entsprechender Log-Eintrag erstellt und das Programm wird beendet.
        var contextResult = WorkflowContext.Create(options.QuickTimeMasterDirectory, options.PublishedMpeg4Directory)
            .Tap(context => Logger.Info($"Executing workflow with quick time masterfile directory: {context.QuickTimeMasterDirectory.Directory}"))
            .Tap(context => Logger.Info($"Executing workflow with published MPEG-4 directory: {context.PublishedMpeg4Directory.Directory}"));
        if (contextResult.IsFailure)
        {
            Logger.Error($"Error creating workflow context: {contextResult.Error}");
            return;
        }

        // Führe die ausgewählte Workflow-Strategie aus. Die Strategien selbst bestehen aus einer Abfolge
        // von Befehlen (Commands), deren Execute-Methode den Workflow implementiert.
        var executionResult = strategyResult.Value.SelectedStrategy.ExecuteAsync(contextResult.Value);

        // Log-Observer abschliessen
        // Diese Methode wird aufgerufen, um dem LogObserver zu signalisieren, dass das Observable, auf das er
        // abonniert hat (in diesem Fall der EventBroadcaster der gewählten Strategie), keine weiteren Daten 
        // senden wird. Nachdem diese Methode aufgerufen wurde, sollte der LogObserver keine weiteren 
        // OnNext- oder OnError-Aufrufe von diesem Observable erwarten. 
        logObserver.OnCompleted();

        // Beende die Verbindung zwischen dem LogObserver und der ausgewählten Workflow-Strategie.
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


/// <summary>
/// Die Options-Klasse enthält die Konfigurationsoptionen für die Ausführung der Konsolenanwendung.
/// </summary>
public class Options
{
    /// <summary>
    /// Pfad zum Verzeichnis, in dem sich die zu bearbeitenden QuickTime-Masterdateien befinden.
    /// </summary>
    public string? QuickTimeMasterDirectory { get; set; }

    /// <summary>
    /// Pfad zum Zielverzeichnis, in das die konvertierten MPEG-4-Dateien sich befinden.
    /// </summary>
    public string? PublishedMpeg4Directory { get; set; }

    /// <summary>
    /// Name der zu verwendenden Workflow-Strategie.
    /// </summary>
    public string? Workflow { get; set; }
}
