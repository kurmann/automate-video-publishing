public interface IObservableCommand<TResult, TEvent>
{
    Task<Result<TResult>> Execute(WorkflowContext context);
    IDisposable Subscribe(IObserver<TEvent> observer);
}
