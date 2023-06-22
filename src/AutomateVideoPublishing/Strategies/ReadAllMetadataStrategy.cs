using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies;

public class ReadAllMetadataStrategy : IWorkflowStrategy
{
    private readonly Subject<string> _broadcaster = new();
    private readonly WriteJsonMetadataCommand _writeCommand;

    public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

    public ReadAllMetadataStrategy()
    {
        var quickTimeCommand = new QuickTimeMetadataReadCommand();
        var mpeg4Command = new Mpeg4DirectoryMetadataReadCommand();
        _writeCommand = new WriteJsonMetadataCommand(quickTimeCommand, mpeg4Command);
    }

    public void Execute(WorkflowContext context)
    {
        _writeCommand.WhenDataAvailable.Subscribe(
            onNext: filepath => _broadcaster.OnNext($"New Json file created: {filepath}"),
            onError: ex => _broadcaster.OnError(ex),
            onCompleted: () => {
                _broadcaster.OnNext("ReadAllMetadataStrategy execution was successful.");
                _broadcaster.OnCompleted();
            }
        );

        _writeCommand.Execute(context);
    }
}
