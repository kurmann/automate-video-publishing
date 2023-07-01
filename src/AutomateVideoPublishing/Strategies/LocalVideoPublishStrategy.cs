using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies;

public class LocalVideoPublishStrategy : IWorkflowStrategy
{
    private Subject<string> _broadcaster = new();

    public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

    public void Execute(WorkflowContext context)
    {
        var atomicParsleyRunCommand = new AtomicParsleyRunCommand();
        var mmeg4MetadataReadCommand = new Mpeg4MetadataReadCommand(atomicParsleyRunCommand);
        var writeMetadataToTextFileCommand = new WriteMetadataToTextFileCommand(mmeg4MetadataReadCommand);

        writeMetadataToTextFileCommand.WhenDataAvailable.Subscribe(
            fileInfo =>
            {
                if (fileInfo != null)
                {
                    _broadcaster.OnNext($"Metdata written to file: {fileInfo.FullName}");
                }
            },
            exception =>
            {
                // Handle any error
                _broadcaster.OnError(exception);
            }
        );

        writeMetadataToTextFileCommand.Execute(context);
    }
}
