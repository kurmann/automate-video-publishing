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
        var pairCollectorCommand = new PairCollectorCommand(metadataReadCommand);
        var metadataTransferCommand = new MetadataTransferCommand(pairCollectorCommand);
        var moveToMediaLocalDirectoryCommand = new MoveToMediaLocalDirectoryCommand();

        metadataTransferCommand.WhenDataAvailable.Subscribe(
            transferredMetadata =>
            {
                if (transferredMetadata != null)
                {
                    _broadcaster.OnNext(transferredMetadata.SummaryMessage);
                    // Nachdem die Metadaten übertragen wurden, führen Sie den Befehl zum Verschieben in das lokale Verzeichnis aus
                    moveToMediaLocalDirectoryCommand.Execute(context);
                }
            },
            exception =>
            {
                // Handle any error
                _broadcaster.OnError(exception);
            }
        );

        moveToMediaLocalDirectoryCommand.WhenDataAvailable.Subscribe(
            result =>
            {
                // Verarbeiten Sie das Ergebnis des moveToMediaLocalDirectoryCommand hier
                // Zum Beispiel könnten Sie eine Nachricht an den _broadcaster senden
                _broadcaster.OnNext($"Move to local directory result: {result}");
            },
            exception =>
            {
                // Handle any error
                _broadcaster.OnError(exception);
            }
        );

        metadataTransferCommand.Execute(context);

        _broadcaster.OnCompleted();
    }
}
