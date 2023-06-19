namespace AutomateVideoPublishing.Strategies;

public interface IWorkflowStrategy<T>
{
    Result<T> Execute(WorkflowContext context);
}
