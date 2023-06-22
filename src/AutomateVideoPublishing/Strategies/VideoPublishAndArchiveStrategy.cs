using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities
{
    public class VideoPublishAndArchiveStrategy : IWorkflowStrategy
    {
        private Subject<string> _broadcaster = new();

        public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

        public void Execute(WorkflowContext context)
        {
            // Führen Sie hier die Übertragung der Metadaten durch.
            // Dies ist momentan ein Platzhalter und muss entsprechend Ihrem Bedarf implementiert werden.

            // Für das Demo, lasst uns eine erfolgreiche Nachricht senden
            _broadcaster.OnNext("TransmitMetadataStrategy execution was successful.");

            // und dann die Fertigstellung melden
            _broadcaster.OnCompleted();
        }
    }
}
