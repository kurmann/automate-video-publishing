using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies
{
    public class VideoPublishAndArchiveStrategy : IWorkflowStrategy
    {
        private Subject<string> _broadcaster = new();

        public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

        public void Execute(WorkflowContext context)
        {
            var metadataReadCommand = new QuickTimeMetadataReadCommand();
            var metadataTransferCommand = new MetadataTransferCommand(metadataReadCommand);

            metadataTransferCommand.WhenDataAvailable.Subscribe(
                transferredMetadata =>
                {
                    if (transferredMetadata != null)
                    {
                        _broadcaster.OnNext(transferredMetadata.SummaryMessage);
                    }
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
}
