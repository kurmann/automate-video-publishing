public interface ICommand<T, TEventArg> where TEventArg : EventArgs
{
    event EventHandler<TEventArg>? CommandProgressChanged;

    Task<Result<T>> Execute(WorkflowContext context);
}
