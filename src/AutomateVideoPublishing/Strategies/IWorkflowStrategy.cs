namespace AutomateVideoPublishing.Strategies;

public interface IWorkflowStrategy
{
    Task ExecuteAsync(WorkflowContext context);

    IObservable<string> EventBroadcaster { get; }
}
