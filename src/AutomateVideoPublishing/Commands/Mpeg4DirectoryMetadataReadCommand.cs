public class Mpeg4DirectoryMetadataReadCommand : ICommand
{
    public event EventHandler<Mpeg4CommandProgressChangedEventArgs>? CommandProgressChanged;

    public Task Execute(WorkflowContext context)
    {
        if (context == null)
        {
            return Task.FromResult(Result.Failure<List<Mpeg4MetadataContainer>>("Workflow context cannot be null."));
        }

        var containers = new List<Mpeg4MetadataContainer>();
        foreach (var mpeg4File in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var containerResult = Mpeg4MetadataContainer.Create(mpeg4File.FullName);
            if (containerResult.IsFailure)
            {
                return Task.FromResult(Result.Failure<List<Mpeg4MetadataContainer>>(containerResult.Error));
            }

            containers.Add(containerResult.Value);
            CommandProgressChanged?.Invoke(this, new Mpeg4CommandProgressChangedEventArgs(containerResult.Value));
        }

        return Task.CompletedTask;
    }
}

public class Mpeg4CommandProgressChangedEventArgs : EventArgs
{
    public Mpeg4MetadataContainer Mpeg4MetadataContainer { get; }

    public Mpeg4CommandProgressChangedEventArgs(Mpeg4MetadataContainer mpeg4MetadataContainer) 
        => Mpeg4MetadataContainer = mpeg4MetadataContainer ?? throw new ArgumentNullException(nameof(mpeg4MetadataContainer));
}
