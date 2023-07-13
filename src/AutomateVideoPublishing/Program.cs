using AutomateVideoPublishing.Workflows;
using Microsoft.Extensions.Configuration;

class Program
{
    // Definiere einen Logger für die Anwendung. Die Konfiguration des Loggers erfolgt 
    // durch die NLog-Konfigurationsdatei (nlog.config).
    private static readonly Logger logger = LogManager.GetCurrentClassLogger();

    // Die Main-Methode ist der Einstiegspunkt der Konsolenanwendung. 
    static async Task Main(string[] args)
    {
        logger.Info("Starting Automate Video Publishing application");

        // Erstelle ein Konfigurationsobjekt, das Umgebungsvariablen und Befehlszeilenargumente
        // berücksichtigt.
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        // Die Optionen aus der Konfiguration werden in ein Options-Objekt gebunden.
        var options = new Options();
        configuration.Bind(options);

        logger.Info($"Parsing workflow name {options.Workflow} from configuration values");
        // Erzeuge eine Instanz der ausgewählten Workflow-Strategie mithilfe eines Strategie-Mappers.
        // Bei einem Fehler wird ein entsprechender Log-Eintrag erstellt und das Programm wird beendet.
        var workflowMapperResult = AsyncWorkflowMapper.Create(options.Workflow);
        if (workflowMapperResult.IsFailure)
        {
            logger.Error($"Error on parsing workflow: {workflowMapperResult.Error}");
            return;
        }

        // Erzeuge den Workflow-Kontext, der Daten enthält, die für alle Workflow-Strategien nützlich sind. 
        // Bei einem Fehler wird ein entsprechender Log-Eintrag erstellt und das Programm wird beendet.
        var contextResult = WorkflowContext.Create(options.QuickTimeMasterDirectory,
                                                   options.PublishedMpeg4Directory,
                                                   options.PublishedMediaLocalDirectory)
            .Tap(context => logger.Info($"Start execution of {workflowMapperResult.Value}."))
            .Tap(context => logger.Info($"Executing workflow with quick time masterfile directory: {context.QuickTimeMasterDirectory.Directory}"))
            .Tap(context => logger.Info($"Executing workflow with published MPEG-4 directory: {context.PublishedMpeg4Directory.Directory}"));
        if (contextResult.IsFailure)
        {
            logger.Error($"Error creating workflow context: {contextResult.Error}");
            return;
        }

        // Führe die ausgewählte Workflow-Strategie aus. Die Strategien selbst bestehen aus einer Abfolge
        // von Befehlen (Commands), deren Execute-Methode den Workflow implementiert.
        var worfklowResult = await workflowMapperResult.Value.ExecuteAsync(contextResult.Value);
        if (worfklowResult.IsSuccess)
        {
            Console.WriteLine("Workflow completed successfully");
            Environment.ExitCode = 0;  // Erfolg
        }
        else
        {
            Console.WriteLine($"Workflow failed: {worfklowResult.Error}");
            Environment.ExitCode = 1;  // Fehler
        }
    }
}


/// <summary>
/// Die Options-Klasse enthält die Konfigurationsoptionen für die Ausführung der Konsolenanwendung.
/// </summary>
public class Options
{
    /// <summary>
    /// Name der zu verwendenden Workflow-Strategie.
    /// </summary>
    public string? Workflow { get; set; }

    /// <summary>
    /// Pfad zum Verzeichnis, in dem sich die zu bearbeitenden QuickTime-Masterdateien befinden.
    /// </summary>
    public string? QuickTimeMasterDirectory { get; set; }

    /// <summary>
    /// Pfad zum Zielverzeichnis, in das die konvertierten MPEG-4-Dateien sich befinden.
    /// </summary>
    public string? PublishedMpeg4Directory { get; set; }

    /// <summary>
    /// Pfad zum lokalen Zielverzeichnis, in das die sortierten MPEG-4-Dateien verschoben werden sollen.
    /// </summary>
    public string? PublishedMediaLocalDirectory { get; set; }
}
