using AutomateVideoPublishing.Commands;
using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities
{
    public class ReadAllMetadataStrategy : IWorkflowStrategy
    {
        public Task Execute(WorkflowContext context)
        {
            var command = new CompositeMetadataReadWithJsonOutputCommand(new CompositeMetadataReadCommand());
            var commandResult = command.Execute(context);

            if (commandResult.CreatedJsonFiles.Any(item => item.IsFailure))
            {
                var failedFiles = commandResult.CreatedJsonFiles
                    .Where(fileResult => fileResult.IsFailure)
                    .Select(fileResult => fileResult.Error);

                var errorMessage = "Failed to create JSON files for the following files: " + string.Join(", ", failedFiles);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

    }
}
