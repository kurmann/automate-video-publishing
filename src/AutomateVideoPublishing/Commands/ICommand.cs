public interface ICommand<T>
{
    Task<Result<T>> Execute();
}
