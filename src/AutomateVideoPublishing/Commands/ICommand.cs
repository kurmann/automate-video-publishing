public interface ICommand
{
    Task Execute(WorkflowContext context);
}