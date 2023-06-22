namespace AutomateVideoPublishing.Commands;

public interface IAsyncCommand<T> : ICommand<T>
{
    Task ExecuteAsync(WorkflowContext context);
}