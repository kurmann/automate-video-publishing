namespace AutomateVideoPublishing.Strategies;

public class WorkflowStrategyMapper
{

    public IWorkflowStrategy SelectedStrategy { get; private set; }

    private WorkflowStrategyMapper(IWorkflowStrategy strategy) => SelectedStrategy = strategy;

    public static Result<WorkflowStrategyMapper, string> Create(string? strategyName)
    {
        if (string.IsNullOrWhiteSpace(strategyName))
        {
            strategyName = nameof(ReadAllMetadataStrategy);
        }

        IWorkflowStrategy strategy;
        switch (strategyName)
        {
            case nameof(ReadAllMetadataStrategy):
                strategy = new ReadAllMetadataStrategy();
                break;
            case nameof(VideoPublishAndArchiveStrategy):
                strategy = new VideoPublishAndArchiveStrategy();
                break;
            case nameof(LocalVideoPublishStrategy):
                strategy = new LocalVideoPublishStrategy();
                break;
            default:
                return Result.Failure<WorkflowStrategyMapper, string>($"Unknown strategy: {strategyName}");
        }

        return Result.Success<WorkflowStrategyMapper, string>(new WorkflowStrategyMapper(strategy));
    }
}
