using System.Text.Json;

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

            foreach (var quickTimeMetadata in commandResult.QuickTimeMetadataContainers)
            {
                File.WriteAllText(Path.ChangeExtension(quickTimeMetadata.File.FullName, ".json"), JsonSerializer.Serialize(quickTimeMetadata.RawMetadata));
            }

            foreach (var mpeg4Metadata in commandResult.Mpeg4MetadataContainers)
            {
                File.WriteAllText(Path.ChangeExtension(mpeg4Metadata.File.FullName, ".json"), JsonSerializer.Serialize(mpeg4Metadata.Tags));
            }

            return commandResult;
        }
    }
}
