namespace AutomateVideoPublishing.Commands;

public interface IResultCommand<T>
{
    Result<T> Execute(WorkflowContext context);
}