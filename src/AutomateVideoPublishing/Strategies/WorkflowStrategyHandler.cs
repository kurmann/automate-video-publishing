using AutomateVideoPublishing.Strategies;

public class WorkflowStrategyHandler<T> : IWorkflowStrategyHandler
{
    private readonly IWorkflowStrategy<T> _strategy;

    public WorkflowStrategyHandler(IWorkflowStrategy<T> strategy)
    {
        _strategy = strategy;
    }

    public Result Execute(WorkflowContext context)
    {
        var result = _strategy.Execute(context);

        // Konvertieren Sie das Result-Objekt in ein Result ohne generischen Typ
        return result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
    }
}