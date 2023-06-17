public class WorkflowContext
{
    private const string DefaultDirectory = ".";  // Aktuelles Verzeichnis

    public DirectoryInfo QuickTimeMasterDirectory { get; }
    public DirectoryInfo PublishedMpeg4Directory { get; }

    private WorkflowContext(DirectoryInfo quickTimeMasterDirectory, DirectoryInfo publishedMpeg4Directory)
    {
        QuickTimeMasterDirectory = quickTimeMasterDirectory;
        PublishedMpeg4Directory = publishedMpeg4Directory;
    }

    public static Result<WorkflowContext, string> Create(string? quickTimeMasterDirectoryPath, string? publishedMpeg4DirectoryPath)
    {
        // Verwenden Sie Standardwerte, wenn keine bereitgestellt werden
        quickTimeMasterDirectoryPath = string.IsNullOrWhiteSpace(quickTimeMasterDirectoryPath) ? DefaultDirectory : quickTimeMasterDirectoryPath;
        publishedMpeg4DirectoryPath = string.IsNullOrWhiteSpace(publishedMpeg4DirectoryPath) ? DefaultDirectory : publishedMpeg4DirectoryPath;

        var quickTimeMasterDirectory = new DirectoryInfo(quickTimeMasterDirectoryPath);
        if (!quickTimeMasterDirectory.Exists)
        {
            return Result.Failure<WorkflowContext, string>("QuickTime master directory does not exist.");
        }

        var publishedMpeg4Directory = new DirectoryInfo(publishedMpeg4DirectoryPath);
        if (!publishedMpeg4Directory.Exists)
        {
            return Result.Failure<WorkflowContext, string>("Published MPEG-4 directory does not exist.");
        }

        // Workflow-Kontext erstellen und zurückgeben
        return Result.Success<WorkflowContext, string>(new WorkflowContext(quickTimeMasterDirectory, publishedMpeg4Directory));
    }
}
