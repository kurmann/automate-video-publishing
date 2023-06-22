public class QuickTimeMetadataReadCommand : ICommand
{
    private EventBroadcaster<QuickTimeMetadataContainer> broadcaster = new();

    public Task ExecuteAsync(WorkflowContext context)
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
                broadcaster.BroadcastError(containerResult.Error);
                return Task.CompletedTask;
            }

            broadcaster.BroadcastNext(containerResult.Value);
        }

        broadcaster.BroadcastCompleted();
        return Task.CompletedTask;
    }

    public IDisposable Subscribe(IObserver<QuickTimeMetadataContainer> observer) => broadcaster.Subscribe(observer);
}
