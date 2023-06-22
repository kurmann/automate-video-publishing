using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies;

public class VideoPublishAndArchiveStrategy : IWorkflowStrategy
{
    private Subject<string> _broadcaster = new();

    public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

    public void Execute(WorkflowContext context)
    {
        var metadataReadCommand = new QuickTimeMetadataReadCommand();
        var metadataTransferCommand = new MetadataTransferCommand(metadataReadCommand);

        metadataTransferCommand.WhenDataAvailable.Subscribe(transferredMetadata =>
        {
            string message = $"Metadaten übertragen für Datei: {transferredMetadata.FileName}, Beschreibung: {transferredMetadata.Description}, Jahr: {transferredMetadata.Year}.";
            _broadcaster.OnNext(message);
        });

        metadataTransferCommand.Execute(context);

        _broadcaster.OnCompleted();
    }
}
