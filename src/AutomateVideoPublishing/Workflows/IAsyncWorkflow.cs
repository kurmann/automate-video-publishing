namespace AutomateVideoPublishing.Workflows;

public interface IAsyncWorkflow
{
    Task<Result> ExecuteAsync(WorkflowContext context);
}