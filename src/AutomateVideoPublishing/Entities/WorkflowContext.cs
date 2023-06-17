using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

public class WorkflowContext
{
    public DirectoryInfo QuickTimeMasterDirectory { get; }
    public DirectoryInfo PublishedMpeg4Directory { get; }

    private WorkflowContext(DirectoryInfo quickTimeMasterDirectory, DirectoryInfo publishedMpeg4Directory)
    {
        QuickTimeMasterDirectory = quickTimeMasterDirectory;
        PublishedMpeg4Directory = publishedMpeg4Directory;
    }

    public static Result<WorkflowContext, string> Create(string quickTimeMasterDirectoryPath, string publishedMpeg4DirectoryPath)
    {
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

        return Result.Success<WorkflowContext, string>(new WorkflowContext(quickTimeMasterDirectory, publishedMpeg4Directory));
    }
}
