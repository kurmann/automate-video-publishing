namespace AutomateVideoPublishing.Strategies;

public interface IWorkflowStrategy
{
    Task ExecuteAsync(WorkflowContext context);

    EventBroadcaster<string> EventBroadcaster { get; }
}
