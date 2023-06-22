public interface ICommand
{
    Task ExecuteAsync(WorkflowContext context);
}