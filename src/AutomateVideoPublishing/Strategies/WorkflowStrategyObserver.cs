public abstract class WorkflowStrategyObserver<T> : IObserver<T>, IObservable<string>
{
    private List<IObserver<string>> observers = new List<IObserver<string>>();
    protected string StrategyName { get; }

    protected WorkflowStrategyObserver(string strategyName) => StrategyName = strategyName;

    public virtual void OnNext(T value)
    {
        // By default, do nothing when OnNext is called.
        // Subclasses can override this method to define specific behavior.
    }

    public void OnError(Exception error) => Broadcast($"An error occurred in {StrategyName}: {error.Message}");

    public void OnCompleted() => Broadcast($"{StrategyName} has completed its tasks.");

    public IDisposable Subscribe(IObserver<string> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber<string>(observers, observer);
    }

    protected void Broadcast(string message)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(message);
        }
    }
}
