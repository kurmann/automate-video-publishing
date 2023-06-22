using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities;

public class TransmitMetadataStrategy : IWorkflowStrategy
{
    private EventBroadcaster<string> _broadcaster = new();

    public EventBroadcaster<string> EventBroadcaster => _broadcaster;

    public async Task Execute(WorkflowContext context)
    {
        // Führen Sie hier die Übertragung der Metadaten durch.
        // Dies ist momentan ein Platzhalter und muss entsprechend Ihrem Bedarf implementiert werden.

        // Für das Demo, lasst uns eine erfolgreiche Nachricht senden
        _broadcaster.BroadcastNext("TransmitMetadataStrategy execution was successful.");

        // und dann die Fertigstellung melden
        _broadcaster.BroadcastCompleted();
        
        // Da es sich um einen Platzhalter handelt, ist die Aufgabe bereits abgeschlossen
        await Task.CompletedTask;
    }
}
