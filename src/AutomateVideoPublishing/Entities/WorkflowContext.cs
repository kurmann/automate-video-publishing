public class WorkflowContext
{
    private const string DefaultDirectory = ".";  // Aktuelles Verzeichnis

    public ValidQuickTimeMasterDirectory QuickTimeMasterDirectory { get; }
    public ValidMpeg4Directory PublishedMpeg4Directory { get; }

    private WorkflowContext(ValidQuickTimeMasterDirectory quickTimeMasterDirectory, ValidMpeg4Directory publishedMpeg4Directory)
    {
        QuickTimeMasterDirectory = quickTimeMasterDirectory;
        PublishedMpeg4Directory = publishedMpeg4Directory;
    }

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
