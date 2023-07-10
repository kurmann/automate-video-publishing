namespace AutomateVideoPublishing.Commands;

public interface IAsyncWorkflow
{
    Task ExecuteAsync(WorkflowContext context);
}