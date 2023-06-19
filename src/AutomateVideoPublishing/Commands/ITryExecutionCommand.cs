namespace AutomateVideoPublishing.Commands;

public interface ITryExecutionCommand<T>
{
    T Execute(WorkflowContext context);
}