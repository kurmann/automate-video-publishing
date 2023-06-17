namespace AutomateVideoPublishing.Strategies;

public interface IWorkflowStrategy
{
    Result Execute(WorkflowContext context);
}
