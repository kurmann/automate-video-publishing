using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies;

public class ReadAllMetadataStrategy : IWorkflowStrategy
{
    private readonly Subject<string> _broadcaster = new();
    private WriteMetadataToTextFileCommand _writeMetadataToTextFileCommand;

    public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

    public ReadAllMetadataStrategy()
    {
        var atomicParsleyCommand =  new AtomicParsleyReadMetadataCommand();
        var mpeg4MetadataReadCommand = new Mpeg4MetadataReadCommand(atomicParsleyCommand);
        _writeMetadataToTextFileCommand = new WriteMetadataToTextFileCommand(mpeg4MetadataReadCommand);
    }

    public void Execute(WorkflowContext context)
    {
        _writeMetadataToTextFileCommand.WhenDataAvailable.Subscribe(
            onNext: filepath => _broadcaster.OnNext($"New Json file created: {filepath}"),
            onError: ex => _broadcaster.OnError(ex),
            onCompleted: () => {
                _broadcaster.OnNext("ReadAllMetadataStrategy execution was successful.");
                _broadcaster.OnCompleted();
            }
        );

        _writeMetadataToTextFileCommand.Execute(context);
    }
}
