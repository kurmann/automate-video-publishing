namespace AutomateVideoPublishing.Strategies;

public class ReadAllMetadataStrategy : IWorkflowStrategy, IObserver<FileInfo>
{
    private EventBroadcaster<string> _broadcaster = new();
    private WriteJsonMetadataCommand _writeCommand;

    public EventBroadcaster<string> EventBroadcaster => _broadcaster;

    public ReadAllMetadataStrategy()
    {
        var quickTimeCommand = new QuickTimeMetadataReadCommand();
        var mpeg4Command = new Mpeg4DirectoryMetadataReadCommand();
        _writeCommand = new WriteJsonMetadataCommand(quickTimeCommand, mpeg4Command);
        _writeCommand.Subscribe(this);
    }

    public async Task ExecuteAsync(WorkflowContext context)
    {
        await _writeCommand.ExecuteAsync(context);
        _broadcaster.BroadcastNext("ReadAllMetadataStrategy execution was successful.");
        _broadcaster.BroadcastCompleted();
    }

    public void OnCompleted() { }

    public void OnError(Exception error) => _broadcaster.BroadcastError(error.Message);

    public void OnNext(FileInfo jsonFile) => _broadcaster.BroadcastNext($"New Json file created: {jsonFile.FullName}");
}
