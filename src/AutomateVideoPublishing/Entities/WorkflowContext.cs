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
    public static Result<WorkflowContext, string> Create(string? quickTimeMasterDirectoryPath, string? publishedMpeg4DirectoryPath)
    {
        // Use default values if none are provided
        bool isQuickTimeMasterDirectoryDefault = string.IsNullOrWhiteSpace(quickTimeMasterDirectoryPath);
        bool isPublishedMpeg4DirectoryDefault = string.IsNullOrWhiteSpace(publishedMpeg4DirectoryPath);
        
        quickTimeMasterDirectoryPath = isQuickTimeMasterDirectoryDefault ? DefaultDirectory : quickTimeMasterDirectoryPath;
        publishedMpeg4DirectoryPath = isPublishedMpeg4DirectoryDefault ? DefaultDirectory : publishedMpeg4DirectoryPath;

        var quickTimeMasterDirectoryResult = ValidQuickTimeMasterDirectory.Create(quickTimeMasterDirectoryPath);
        if (quickTimeMasterDirectoryResult.IsFailure)
        {
            string errorContext = isQuickTimeMasterDirectoryDefault ?
                "The default QuickTime master directory does not exist or is not valid." :
                "The provided QuickTime master directory does not exist or is not valid.";
            return Result.Failure<WorkflowContext, string>(errorContext);
        }

        var publishedMpeg4DirectoryResult = ValidMpeg4Directory.Create(publishedMpeg4DirectoryPath);
        if (publishedMpeg4DirectoryResult.IsFailure)
        {
            string errorContext = isPublishedMpeg4DirectoryDefault ?
                "The default published MPEG-4 directory does not exist or is not valid." :
                "The provided published MPEG-4 directory does not exist or is not valid.";
            return Result.Failure<WorkflowContext, string>(errorContext);
        }

        // Create and return the workflow context
        return Result.Success<WorkflowContext, string>(new WorkflowContext(quickTimeMasterDirectoryResult.Value, publishedMpeg4DirectoryResult.Value));
    }
}
