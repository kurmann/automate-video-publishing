using AutomateVideoPublishing.Commands;
using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities
{
    public class ReadAllMetadataStrategy : IWorkflowStrategy<List<FileInfo>>
    {
        public Result<List<FileInfo>> Execute(WorkflowContext context)
        {
            var command = new CompositeMetadataReadWithJsonOutputCommand(new CompositeMetadataReadCommand());
            var commandResult = command.Execute(context);

            if (commandResult.CreatedJsonFiles.Any(item => item.IsFailure))
            {
                var failedFiles = commandResult.CreatedJsonFiles
                    .Where(fileResult => fileResult.IsFailure)
                    .Select(fileResult => fileResult.Error);

                var errorMessage = "Failed to create JSON files for the following files: " + string.Join(", ", failedFiles);
                return Result.Failure<List<FileInfo>>(errorMessage);
            }

            return commandResult.CreatedJsonFiles.Where(fileResult => fileResult.IsSuccess).Select(file => file.Value).ToList();
        }

    }
}
