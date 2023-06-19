namespace AutomateVideoPublishing.Commands;

public class CompositeMetadataReadCommand : ITryExecuteCommand<CompositeMetadataReadCommandResult>
{
    public CompositeMetadataReadCommandResult Execute(WorkflowContext context)
    {
        if (context == null)
        {
            return new CompositeMetadataReadCommandResult { GeneralMessage = "No files processed. Workflow context cannot be null." };
        }

        var commandResult = new CompositeMetadataReadCommandResult();
        foreach (var quickTimeFile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            commandResult.QuickTimeMetadataContainers.Add(QuickTimeMetadataContainer.Create(quickTimeFile.FullName));
        }
        foreach (var mpeg4File in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            commandResult.Mpeg4MetadataContainers.Add(Mpeg4MetadataContainer.Create(mpeg4File.FullName));
        }

        if (commandResult.QuickTimeMetadataContainers.Any(c => c.IsFailure) || commandResult.Mpeg4MetadataContainers.Any(c => c.IsFailure))
        {
            commandResult.GeneralMessage = "Could not read all metadata from directories. See individual container results for details.";
        }

        return commandResult;
    }
}
