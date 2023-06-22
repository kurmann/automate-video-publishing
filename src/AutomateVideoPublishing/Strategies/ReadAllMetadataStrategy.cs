using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Strategies;

public class ReadAllMetadataStrategy : IWorkflowStrategy, IObserver<FileInfo>
{
    private Subject<string> _broadcaster = new();
    private WriteJsonMetadataCommand _writeCommand;

    public IObservable<string> EventBroadcaster => _broadcaster.AsObservable();

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
        _broadcaster.OnNext("ReadAllMetadataStrategy execution was successful.");
        _broadcaster.OnCompleted();
    }

    public void OnCompleted() { }

    public void OnError(Exception error) => _broadcaster.OnError(error);

    public void OnNext(FileInfo jsonFile) => _broadcaster.OnNext($"New Json file created: {jsonFile.FullName}");
}
