namespace AutomateVideoPublishing.Commands;

public interface IAsyncCommand<TInput, TResult>
{
    Task<TResult> ExecuteAsync(TInput input);
}