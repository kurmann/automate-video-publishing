namespace AutomateVideoPublishing.Strategies;

public interface IWorkflowStrategy
{
    void Execute(WorkflowContext context);

    IObservable<string> WhenStatusUpdateAvailable { get; }
}
