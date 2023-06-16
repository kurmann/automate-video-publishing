using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities;

public class TransmitMetadataStrategy : IWorkflowStrategy
{
    public void Execute(WorkflowContext context)
    {
        // Führen Sie hier die Übertragung der Metadaten durch.
        // Dies ist momentan ein Platzhalter und muss entsprechend Ihrem Bedarf implementiert werden.
        Console.WriteLine("Transmitting metadata from {0} to {1}", context.QuickTimeMasterDirectory, context.PublishedMpeg4Directory);
    }
}
