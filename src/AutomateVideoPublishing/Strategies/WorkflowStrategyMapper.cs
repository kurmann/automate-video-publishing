namespace AutomateVideoPublishing.Strategies;

public class WorkflowStrategyMapper
{
    public const string ReadAllMetadataStrategy = nameof(ReadAllMetadataStrategy);
    public const string VideoPublishAndArchiveStrategy = nameof(VideoPublishAndArchiveStrategy);

    public IWorkflowStrategy SelectedStrategy { get; private set; }

    private WorkflowStrategyMapper(IWorkflowStrategy strategy) => SelectedStrategy = strategy;

    public static Result<WorkflowStrategyMapper, string> Create(string? strategyName)
    {
        if (string.IsNullOrWhiteSpace(strategyName))
        {
            strategyName = ReadAllMetadataStrategy;
        }

        IWorkflowStrategy strategy;
        switch (strategyName)
        {
            case ReadAllMetadataStrategy:
                strategy = new ReadAllMetadataStrategy();
                break;
            case VideoPublishAndArchiveStrategy:
                strategy = new VideoPublishAndArchiveStrategy();
                break;
            default:
                return Result.Failure<WorkflowStrategyMapper, string>($"Unknown strategy: {strategyName}");
        }

        return Result.Success<WorkflowStrategyMapper, string>(new WorkflowStrategyMapper(strategy));
    }
}
