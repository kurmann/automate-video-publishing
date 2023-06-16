using AutomateVideoPublishing.Strategies;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

public class TransmitMetadataStrategy : IWorkflowStrategy
{
    public Result<WorkflowContext> Execute(WorkflowContext context)
    {
        // Führen Sie hier die Übertragung der Metadaten durch.
        // Dies ist momentan ein Platzhalter und muss entsprechend Ihrem Bedarf implementiert werden.
        Console.WriteLine("Transmitting metadata from {0} to {1}", context.QuickTimeMasterDirectory, context.PublishedMpeg4Directory);
        return Result.Success<WorkflowContext>(context);
    }
}
