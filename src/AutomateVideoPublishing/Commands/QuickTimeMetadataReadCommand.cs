public class QuickTimeMetadataReadCommand : IObservable<QuickTimeMetadataContainer>, ICommand
{
    private List<IObserver<QuickTimeMetadataContainer>> observers = new();

    public Task Execute(WorkflowContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        foreach (var quickTimeFile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            var containerResult = QuickTimeMetadataContainer.Create(quickTimeFile.FullName);
            if (containerResult.IsFailure)
            {
                NotifyObserversError(containerResult.Error);
                return Task.CompletedTask;
            }

            NotifyObserversNext(containerResult.Value);
        }

        NotifyObserversCompleted();
        return Task.CompletedTask;
    }

    public IDisposable Subscribe(IObserver<QuickTimeMetadataContainer> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber<QuickTimeMetadataContainer>(observers, observer);
    }

    private void NotifyObserversNext(QuickTimeMetadataContainer container)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(container);
        }
    }

    private void NotifyObserversError(string error)
    {
        foreach (var observer in observers)
        {
            observer.OnError(new Exception(error));
        }
    }

    private void NotifyObserversCompleted()
    {
        foreach (var observer in observers)
        {
            observer.OnCompleted();
        }
    }
}
