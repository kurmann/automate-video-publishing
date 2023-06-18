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
        // Verwenden Sie Standardwerte, wenn keine bereitgestellt werden
        quickTimeMasterDirectoryPath = string.IsNullOrWhiteSpace(quickTimeMasterDirectoryPath) ? DefaultDirectory : quickTimeMasterDirectoryPath;
        publishedMpeg4DirectoryPath = string.IsNullOrWhiteSpace(publishedMpeg4DirectoryPath) ? DefaultDirectory : publishedMpeg4DirectoryPath;

        var quickTimeMasterDirectoryResult = ValidQuickTimeMasterDirectory.Create(quickTimeMasterDirectoryPath);
        if (quickTimeMasterDirectoryResult.IsFailure)
        {
            return Result.Failure<WorkflowContext, string>("QuickTime master directory does not exist or is not valid.");
        }

        var publishedMpeg4DirectoryResult = ValidMpeg4Directory.Create(publishedMpeg4DirectoryPath);
        if (publishedMpeg4DirectoryResult.IsFailure)
        {
            return Result.Failure<WorkflowContext, string>("Published MPEG-4 directory does not exist or is not valid.");
        }

        // Workflow-Kontext erstellen und zurückgeben
        return Result.Success<WorkflowContext, string>(new WorkflowContext(quickTimeMasterDirectoryResult.Value, publishedMpeg4DirectoryResult.Value));
    }
}
