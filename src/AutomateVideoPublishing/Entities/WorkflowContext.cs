using Microsoft.Extensions.Logging;

namespace AutomateVideoPublishing.Entities;

public class WorkflowContext
{
    /// <summary>
    /// Das Standardverzeichnis falls QuickTimeMasterDirectory und/oder PublishedMpeg4Directory keine Werte übergeben werden
    /// bei der Instanziierung.
    /// </summary>
    public const string DefaultDirectory = ".";  // Aktuelles Verzeichnis

    /// <summary>
    /// Das Verzeichnis indem sich die QuickTime MOV-Masterdateien befinden.
    /// </summary>
    /// <value></value>
    public ValidQuickTimeMasterDirectory QuickTimeMasterDirectory { get; }

    /// <summary>
    /// Das Verzeichnis der MPEG-4 Dateien, die aus der Masterdatei komprimiert wurden.
    /// </summary>
    /// <value></value>
    public ValidMpeg4Directory PublishedMpeg4Directory { get; }

    /// <summary>
    /// Das lokale Zielverzeichnis, in das die sortierten MPEG-4-Dateien verschoben werden sollen.
    /// </summary>
    public ValidMediaLocalDirectory PublishedMediaLocalDirectory { get; }

    /// <summary>
    /// Die Logger-Instanz
    /// </summary>
    /// <value></value>
    public ILogger Logger { get; }

    private WorkflowContext(
        ValidQuickTimeMasterDirectory quickTimeMasterDirectory,
        ValidMpeg4Directory publishedMpeg4Directory,
        ValidMediaLocalDirectory publishedMediaLocalDirectory,
        ILogger logger)
    {
        QuickTimeMasterDirectory = quickTimeMasterDirectory;
        PublishedMpeg4Directory = publishedMpeg4Directory;
        PublishedMediaLocalDirectory = publishedMediaLocalDirectory;
        Logger = logger;
    }

    /// <summary>
    /// Erstellt einen neuen invarianten Workflow-Context
    /// </summary>
    public static Result<WorkflowContext> Create(
        string? quickTimeMasterDirectoryPath = "",
        string? publishedMpeg4DirectoryPath = "",
        string? publishedMediaLocalDirectoryPath = "",
        ILogger? logger = null)
    {
        // set values or default
        quickTimeMasterDirectoryPath = string.IsNullOrWhiteSpace(quickTimeMasterDirectoryPath) ? DefaultDirectory : quickTimeMasterDirectoryPath;
        publishedMpeg4DirectoryPath = string.IsNullOrWhiteSpace(publishedMpeg4DirectoryPath) ? DefaultDirectory : publishedMpeg4DirectoryPath;
        publishedMediaLocalDirectoryPath = string.IsNullOrWhiteSpace(publishedMediaLocalDirectoryPath) ? Path.Combine(DefaultDirectory, "Published") :publishedMediaLocalDirectoryPath;

        // Set default logger if none is provided
        if (logger == null)
        {
            var loggerFactory = LoggerFactory.Create(builder => 
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });

            logger = loggerFactory.CreateLogger<WorkflowContext>();
        }

        var quickTimeMasterDirectoryResult = ValidQuickTimeMasterDirectory.Create(quickTimeMasterDirectoryPath);

        if (quickTimeMasterDirectoryResult.IsFailure)
        {
            return Result.Failure<WorkflowContext>(quickTimeMasterDirectoryResult.Error);
        }

        var publishedMpeg4DirectoryResult = ValidMpeg4Directory.Create(publishedMpeg4DirectoryPath);
        if (publishedMpeg4DirectoryResult.IsFailure)
        {
            return Result.Failure<WorkflowContext>(publishedMpeg4DirectoryResult.Error);
        }

        var publishedMediaLocalDirectoryResult = ValidMediaLocalDirectory.Create(publishedMediaLocalDirectoryPath);
        if (publishedMediaLocalDirectoryResult.IsFailure)
        {
            return Result.Failure<WorkflowContext>(publishedMediaLocalDirectoryResult.Error);
        }

        return new WorkflowContext(
            quickTimeMasterDirectoryResult.Value,
            publishedMpeg4DirectoryResult.Value,
            publishedMediaLocalDirectoryResult.Value,
            logger);
    }
}
