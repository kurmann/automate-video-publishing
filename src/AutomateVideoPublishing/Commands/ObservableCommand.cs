public interface IObservableCommand<T> : IObservable<T>
{
    Task<Result<T>> Execute(WorkflowContext context);
}
