namespace AutomateVideoPublishing.Strategies;

public interface IWorkflowStrategy
{
    Task Execute(WorkflowContext context);

    EventBroadcaster<string> EventBroadcaster { get; }
}
