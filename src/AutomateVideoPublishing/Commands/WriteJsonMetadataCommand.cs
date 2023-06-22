public class WriteJsonMetadataCommand : ICommand, IObserver<QuickTimeMetadataContainer>, IObserver<Mpeg4MetadataContainer>
{
    private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
    private readonly Mpeg4DirectoryMetadataReadCommand _mpeg4Command;
    private readonly EventBroadcaster<FileInfo> _broadcaster;

    public WriteJsonMetadataCommand(
        QuickTimeMetadataReadCommand quickTimeCommand,
        Mpeg4DirectoryMetadataReadCommand mpeg4Command)
    {
        _quickTimeCommand = quickTimeCommand;
        _mpeg4Command = mpeg4Command;
        _broadcaster = new EventBroadcaster<FileInfo>();
    }

    public async Task ExecuteAsync(WorkflowContext context)
    {
        if (context == null)
        {
            _broadcaster.BroadcastError("Workflow context cannot be null.");
            return;
        }

        _quickTimeCommand.Subscribe(this);
        _mpeg4Command.Subscribe(this);
        
        await _quickTimeCommand.ExecuteAsync(context);
        await _mpeg4Command.ExecuteAsync(context);
        
        _broadcaster.BroadcastCompleted();
    }

    public IDisposable Subscribe(IObserver<FileInfo> observer) => _broadcaster.Subscribe(observer);

    public void OnCompleted() { }

    public void OnError(Exception error) => _broadcaster.BroadcastError(error.Message);

    public void OnNext(QuickTimeMetadataContainer container) => WriteJsonAsync(container.FileInfo);

    public void OnNext(Mpeg4MetadataContainer container) => WriteJsonAsync(container.FileInfo);

    private async Task WriteJsonAsync(FileInfo fileInfo)
    {
        var jsonResult = MediaMetadataJson.Create(fileInfo.FullName);
        if (jsonResult.IsSuccess)
        {
            var jsonFile = new FileInfo(Path.ChangeExtension(fileInfo.FullName, ".json"));
            await File.WriteAllTextAsync(jsonFile.FullName, jsonResult.Value.Json);
            _broadcaster.BroadcastNext(jsonFile);
        }
        else
        {
            _broadcaster.BroadcastError($"Json file from file {fileInfo} could not be generated.");
        }
    }
}
