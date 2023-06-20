public class QuickTimeMetadataReadCommand : ICommand<List<QuickTimeMetadataContainer>, QuickTimeCommandProgressChangedEventArgs>
{
    public event EventHandler<QuickTimeCommandProgressChangedEventArgs>? CommandProgressChanged;

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
            CommandProgressChanged?.Invoke(this, new QuickTimeCommandProgressChangedEventArgs(containerResult.Value));
        }

        return Task.FromResult(Result.Success(containers));
    }
}

public class QuickTimeCommandProgressChangedEventArgs : EventArgs
{
    public QuickTimeMetadataContainer QuickTimeMetadataContainer { get; }

    public QuickTimeCommandProgressChangedEventArgs(QuickTimeMetadataContainer quickTimeMetadataContainer)
        => QuickTimeMetadataContainer = quickTimeMetadataContainer ?? throw new ArgumentNullException(nameof(quickTimeMetadataContainer));
}
