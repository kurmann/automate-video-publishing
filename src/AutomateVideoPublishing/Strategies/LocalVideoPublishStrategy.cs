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
        var atomicParsleyCommand = new AtomicParsleyReadMetadataCommand();
        var mmeg4MetadataReadCommand = new Mpeg4MetadataReadCommand(atomicParsleyCommand);
        var writeMetadataToTextFileCommand = new WriteMetadataToTextFileCommand(mmeg4MetadataReadCommand);
        var updateMetadataCommand = new UpdateMetadataCommand(new CollectMetadataToUpdateCommand(mmeg4MetadataReadCommand));

        writeMetadataToTextFileCommand.WhenDataAvailable.Subscribe(
            fileInfo =>
            {
                if (fileInfo != null)
                {
                    _broadcaster.OnNext($"Read metdata written to file: {fileInfo.FullName}");
                }
            },
            exception =>
            {
                // Handle any error
                _broadcaster.OnError(exception);
            }
        );

        updateMetadataCommand.WhenDataAvailable.Subscribe(
            updateMetadataResult =>
            {
                if (updateMetadataResult != null)
                {
                    _broadcaster.OnNext($"Metdata updated: {updateMetadataResult.SummaryMessage}");
                }
            },
            exception =>
            {
                // Handle any error
                _broadcaster.OnError(exception);
            }
        );

        // writeMetadataToTextFileCommand.Execute(context);
        updateMetadataCommand.Execute(context);
    }
}
