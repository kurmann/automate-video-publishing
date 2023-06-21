public class Mpeg4DirectoryMetadataReadCommand : ICommand
{
    private EventBroadcaster<Mpeg4MetadataContainer> broadcaster = new EventBroadcaster<Mpeg4MetadataContainer>();

    public Task Execute(WorkflowContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        foreach (var mpeg4File in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var containerResult = Mpeg4MetadataContainer.Create(mpeg4File.FullName);
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

    public IDisposable Subscribe(IObserver<Mpeg4MetadataContainer> observer) => broadcaster.Subscribe(observer);
}
