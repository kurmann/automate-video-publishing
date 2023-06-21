public class ReadAllMetadataStrategyObserver : IObserver<FileInfo>, IObservable<string>
{
    private List<IObserver<string>> observers = new List<IObserver<string>>();

    public void OnNext(FileInfo value)
    {
        Broadcast($"New Json file created: {value.FullName}");
    }

    public void OnError(Exception error)
    {
        Broadcast($"An error occured: {error.Message}");
    }

    public void OnCompleted()
    {
        Broadcast("Strategy has completed its task.");
    }

    public IDisposable Subscribe(IObserver<string> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber<string>(observers, observer);
    }

    private void Broadcast(string message)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(message);
        }
    }
}
