namespace AutomateVideoPublishing.Strategies;

public class WorkflowStrategyMapper
{
    public const string ReadAllMetadataStrategy = nameof(ReadAllMetadataStrategy);
    public const string TransmitMetadataStrategy = nameof(TransmitMetadataStrategy);

    public IWorkflowStrategyHandler SelectedStrategyHandler { get; private set; }

    private WorkflowStrategyMapper(IWorkflowStrategyHandler strategyHandler) => SelectedStrategyHandler = strategyHandler;

    public static Result<WorkflowStrategyMapper, string> Create(string? strategyName)
    {
        if (string.IsNullOrWhiteSpace(strategyName))
        {
            strategyName = ReadAllMetadataStrategy;
        }

        IWorkflowStrategyHandler strategyHandler;
        switch (strategyName)
        {
            case ReadAllMetadataStrategy:
                strategyHandler = new WorkflowStrategyHandler<List<FileInfo>>(new ReadAllMetadataStrategy());
                break;
            case TransmitMetadataStrategy:
                // Für dieses Beispiel werde ich davon ausgehen, dass TransmitMetadataStrategy einen bool zurückgibt
                strategyHandler = new WorkflowStrategyHandler<string>(new TransmitMetadataStrategy());
                break;
            default:
                return Result.Failure<WorkflowStrategyMapper, string>($"Unknown strategy: {strategyName}");
        }

        return Result.Success<WorkflowStrategyMapper, string>(new WorkflowStrategyMapper(strategyHandler));
    }
}
