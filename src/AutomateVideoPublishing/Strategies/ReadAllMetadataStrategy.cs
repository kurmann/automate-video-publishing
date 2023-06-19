using AutomateVideoPublishing.Strategies;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Entities;

public class ReadAllMetadataStrategy : IWorkflowStrategy
{
    public Result Execute(WorkflowContext context)
    {
        var command = new CompositeMetadataReadCommand();
        var commandResult = command.Execute(context);

        if (commandResult.FailedFiles.Any()){
            return Result.Failure(commandResult.GeneralMessage);
        }
        return Result.Success("Workflow completed");
    }
}
