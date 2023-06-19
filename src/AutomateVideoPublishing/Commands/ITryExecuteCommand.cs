namespace AutomateVideoPublishing.Commands;

public interface ITryExecuteCommand<T>
{
    T Execute(WorkflowContext context);
}