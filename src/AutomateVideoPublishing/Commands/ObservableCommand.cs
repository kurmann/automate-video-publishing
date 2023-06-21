public interface IObservableCommand<TEvent> : IObservable<TEvent>
{
    Task Execute(WorkflowContext context);
}
