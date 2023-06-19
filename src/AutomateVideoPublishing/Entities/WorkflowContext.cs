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

    private WorkflowContext(ValidQuickTimeMasterDirectory quickTimeMasterDirectory, ValidMpeg4Directory publishedMpeg4Directory)
    {
        QuickTimeMasterDirectory = quickTimeMasterDirectory;
        PublishedMpeg4Directory = publishedMpeg4Directory;
    }

    /// <summary>
    /// Erstellt einen neuen invarianten Workflow-Context
    /// </summary>
    /// <param name="quickTimeMasterDirectoryPath">Das Verzeichnis indem sich die QuickTime MOV-Masterdateien befinden.</param>
    /// <param name="publishedMpeg4DirectoryPath">Das Standardverzeichnis falls QuickTimeMasterDirectory und/oder PublishedMpeg4Directory.</param>
    /// <returns></returns>
    public static Result<WorkflowContext> Create(string? quickTimeMasterDirectoryPath = "", string? publishedMpeg4DirectoryPath = "")
    {
        // set values or default
        quickTimeMasterDirectoryPath = string.IsNullOrWhiteSpace(quickTimeMasterDirectoryPath) ? DefaultDirectory : quickTimeMasterDirectoryPath;
        publishedMpeg4DirectoryPath = string.IsNullOrWhiteSpace(publishedMpeg4DirectoryPath) ? DefaultDirectory : publishedMpeg4DirectoryPath;

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

        return new WorkflowContext(quickTimeMasterDirectoryResult.Value, publishedMpeg4DirectoryResult.Value);
    }
}
