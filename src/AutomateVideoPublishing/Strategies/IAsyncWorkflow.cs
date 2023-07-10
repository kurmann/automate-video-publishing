namespace AutomateVideoPublishing.Strategies;

public interface IAsyncWorkflow
{
    Task ExecuteAsync(WorkflowContext context);
}