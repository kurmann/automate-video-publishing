namespace AutomateVideoPublishing.Commands
{
    public class CompositeMetadataReadWithJsonOutputCommand : ITryExecutionCommand<CompositeMetadataReadCommandResult>
    {
        private readonly CompositeMetadataReadCommand _compositeMetadataReadCommand;

        public CompositeMetadataReadWithJsonOutputCommand(CompositeMetadataReadCommand compositeMetadataReadCommand)
            => _compositeMetadataReadCommand = compositeMetadataReadCommand;

        public CompositeMetadataReadCommandResult Execute(WorkflowContext context)
        {
            var commandResult = _compositeMetadataReadCommand.Execute(context);

            if (commandResult.FailedFiles.Any())
            {
                return commandResult;
            }

            foreach (var file in commandResult.QuickTimeMetadataContainers.Select(qt => qt.FileInfo)
                .Concat(commandResult.Mpeg4MetadataContainers.Select(mp => mp.FileInfo)))
            {
                var jsonResult = MediaMetadataJson.Create(file.FullName);

                if (jsonResult.IsSuccess)
                {
                    var jsonFile = new FileInfo(Path.ChangeExtension(file.FullName, ".json"));
                    File.WriteAllText(jsonFile.FullName, jsonResult.Value.Json);
                    commandResult.CreatedJsonFiles.Add(jsonFile);
                }
                else
                {
                    commandResult.FailedFiles.Add(file, jsonResult.Error);
                }
            }

            return commandResult;
        }
    }
}
