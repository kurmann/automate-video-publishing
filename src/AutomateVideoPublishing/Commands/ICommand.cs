namespace AutomateVideoPublishing.Commands;

public interface ICommand<T>
{
    void Execute(WorkflowContext context);
    IObservable<T> WhenDataAvailable { get; }
}
