namespace AutomateVideoPublishing.Commands;

public interface ICommand<TInput, TResult>
{
    Task<TResult> ExecuteAsync(TInput input);
}