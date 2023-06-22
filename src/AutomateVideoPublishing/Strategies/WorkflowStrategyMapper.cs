namespace AutomateVideoPublishing.Strategies;

public class WorkflowStrategyMapper
{
    public const string ReadAllMetadataStrategy = nameof(ReadAllMetadataStrategy);
    public const string TransmitMetadataStrategy = nameof(TransmitMetadataStrategy);

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
            case TransmitMetadataStrategy:
                // Für dieses Beispiel gehe ich davon aus, dass TransmitMetadataStrategy einen bool zurückgibt
                strategy = new TransmitMetadataStrategy();
                break;
            default:
                return Result.Failure<WorkflowStrategyMapper, string>($"Unknown strategy: {strategyName}");
        }

        return Result.Success<WorkflowStrategyMapper, string>(new WorkflowStrategyMapper(strategy));
    }
}
