namespace AutomateVideoPublishing.Commands;

public class CompositeMetadataReadWithJsonOutputCommand : ITryExecuteCommand<CompositeMetadataReadCommandResult>
{
    private readonly CompositeMetadataReadCommand _compositeMetadataReadCommand;

    public CompositeMetadataReadWithJsonOutputCommand(CompositeMetadataReadCommand compositeMetadataReadCommand)
        => _compositeMetadataReadCommand = compositeMetadataReadCommand;

    public CompositeMetadataReadCommandResult Execute(WorkflowContext context)
    {
        var commandResult = _compositeMetadataReadCommand.Execute(context);

        foreach (var containerResult in commandResult.QuickTimeMetadataContainers)
        {
            if (containerResult.IsSuccess)
            {
                var jsonResult = MediaMetadataJson.Create(containerResult.Value.FileInfo.FullName);

                if (jsonResult.IsSuccess)
                {
                    var jsonFile = new FileInfo(Path.ChangeExtension(containerResult.Value.FileInfo.FullName, ".json"));
                    File.WriteAllText(jsonFile.FullName, jsonResult.Value.Json);
                    commandResult.CreatedJsonFiles.Add(jsonFile);
                }
                else
                {
                    var error = $"Json file from quicktime file {containerResult.Value.FileInfo} could not be generated.";
                    commandResult.CreatedJsonFiles.Add(Result.Failure<FileInfo>(error));
                }
            }
        }

        foreach (var containerResult in commandResult.Mpeg4MetadataContainers)
        {
            if (containerResult.IsSuccess)
            {
                var jsonResult = MediaMetadataJson.Create(containerResult.Value.FileInfo.FullName);

                if (jsonResult.IsSuccess)
                {
                    var jsonFile = new FileInfo(Path.ChangeExtension(containerResult.Value.FileInfo.FullName, ".json"));
                    File.WriteAllText(jsonFile.FullName, jsonResult.Value.Json);
                    commandResult.CreatedJsonFiles.Add(jsonFile);
                }
                else
                {
                    var error = $"Json file from MPEG-4 file {containerResult.Value.FileInfo} could not be generated.";
                    commandResult.CreatedJsonFiles.Add(Result.Failure<FileInfo>(error));
                }
            }
        }

        if (commandResult.QuickTimeMetadataContainers.Any(result => result.IsFailure) ||
            commandResult.Mpeg4MetadataContainers.Any(result => result.IsFailure))
        {
            commandResult.GeneralMessage = "Could not create all JSON files from metadata containers. See individual file results for details.";
        }

        return commandResult;
    }
}
