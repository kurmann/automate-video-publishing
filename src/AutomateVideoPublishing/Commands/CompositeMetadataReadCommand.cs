namespace AutomateVideoPublishing.Commands;

public class CompositeMetadataReadCommand : ITryExecutionCommand<CompositeMetadataReadCommandResult>
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
            var metadataContainerResult = QuickTimeMetadataContainer.Create(quickTimeFile.FullName);
            if (metadataContainerResult.IsFailure)
            {
                commandResult.FailedFiles.Add(quickTimeFile, metadataContainerResult.Error);
            }
            else
            {
                commandResult.QuickTimeMetadataContainers.Add(metadataContainerResult.Value);
            }
        }
        foreach (var mpeg4File in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var metadataContainerResult = Mpeg4MetadataContainer.Create(mpeg4File.FullName);
            if (metadataContainerResult.IsFailure)
            {
                commandResult.FailedFiles.Add(mpeg4File, metadataContainerResult.Error);
            }
            else
            {
                commandResult.Mpeg4MetadataContainers.Add(metadataContainerResult.Value);
            }
        }

        if (commandResult.FailedFiles.Any())
        {
            commandResult.GeneralMessage = "Could not read all metadata from directories See failed files list for details.";
            return commandResult;
        }

        return commandResult;
    }
}

public class CompositeMetadataReadCommandResult
{
    public string? GeneralMessage { get; set; }
    public List<QuickTimeMetadataContainer> QuickTimeMetadataContainers { get; set; } = new List<QuickTimeMetadataContainer>();
    public List<Mpeg4MetadataContainer> Mpeg4MetadataContainers { get; set; } = new List<Mpeg4MetadataContainer>();
    public Dictionary<FileInfo, string> FailedFiles { get; set; } = new Dictionary<FileInfo, string>();

}