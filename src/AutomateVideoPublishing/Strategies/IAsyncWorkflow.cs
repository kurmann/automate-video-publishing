namespace AutomateVideoPublishing.Strategies;

public interface IAsyncWorkflow
{
    Task<Result> ExecuteAsync(WorkflowContext context);
}