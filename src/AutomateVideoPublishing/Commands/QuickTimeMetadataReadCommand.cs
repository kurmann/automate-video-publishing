public class QuickTimeMetadataReadCommand : IObservableCommand<List<QuickTimeMetadataContainer>, QuickTimeMetadataContainer>
{
    private List<IObserver<QuickTimeMetadataContainer>> observers = new();

    public Task<Result<List<QuickTimeMetadataContainer>>> Execute(WorkflowContext context)
    {
        if (context == null)
        {
            return Task.FromResult(Result.Failure<List<QuickTimeMetadataContainer>>("Workflow context cannot be null."));
        }

        var containers = new List<QuickTimeMetadataContainer>();
        foreach (var quickTimeFile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            var containerResult = QuickTimeMetadataContainer.Create(quickTimeFile.FullName);
            if (containerResult.IsFailure)
            {
                return Task.FromResult(Result.Failure<List<QuickTimeMetadataContainer>>(containerResult.Error));
            }

            containers.Add(containerResult.Value);
            NotifyObservers(containerResult.Value); // added to notify observers
        }

        return Task.FromResult(Result.Success(containers));
    }

    public IDisposable Subscribe(IObserver<QuickTimeMetadataContainer> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber<QuickTimeMetadataContainer>(observers, observer);
    }

    private void NotifyObservers(QuickTimeMetadataContainer container)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(container);
        }
    }
}