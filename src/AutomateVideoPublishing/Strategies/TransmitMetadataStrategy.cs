using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities;

public class TransmitMetadataStrategy : IWorkflowStrategy
{
    public Task Execute(WorkflowContext context)
    {
        // Führen Sie hier die Übertragung der Metadaten durch.
        // Dies ist momentan ein Platzhalter und muss entsprechend Ihrem Bedarf implementiert werden.
        return Task.CompletedTask;
    }
}
