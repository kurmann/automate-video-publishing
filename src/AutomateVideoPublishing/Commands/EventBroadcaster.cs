public class EventBroadcaster<T> : IObservable<T>
{
    private List<IObserver<T>> observers = new();

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber<T>(observers, observer);
    }

    public void BroadcastNext(T value)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(value);
        }
    }

    public void BroadcastError(string error)
    {
        foreach (var observer in observers)
        {
            observer.OnError(new Exception(error));
        }
    }

    public void BroadcastCompleted()
    {
        foreach (var observer in observers)
        {
            observer.OnCompleted();
        }
    }
}
