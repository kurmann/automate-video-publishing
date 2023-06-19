using AutomateVideoPublishing.Strategies;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Die Klasse 'WorkflowStrategyMapper' ermöglicht das Erstellen von IWorkflowStrategy-Objekten basierend auf dem Namen einer Strategie.
/// </summary>
public class WorkflowStrategyMapper
{
    // Map der unterstützten Strategien. 
    // Der Schlüssel ist der Name der Strategie und der Wert ist das zugehörige IWorkflowStrategy-Objekt.
    private static readonly Dictionary<string, IWorkflowStrategy> StrategyMap = new()
    {
        { nameof(ReadAllMetadataStrategy), new  ReadAllMetadataStrategy() },
        { nameof(TransmitMetadataStrategy), new TransmitMetadataStrategy() }
        // Hier können weitere Strategien hinzugefügt werden.
    };

    // Die Standardstrategie, die verwendet wird, wenn keine spezifische Strategie angegeben wird.
    public const string DefaultStrategy = nameof(ReadAllMetadataStrategy);

    // Die aktuell gewählte Strategie.
    public IWorkflowStrategy SelectedStrategy { get; private set; }

    private WorkflowStrategyMapper(IWorkflowStrategy strategy) => SelectedStrategy = strategy;

    /// <summary>
    /// Erstellt ein neues 'WorkflowStrategyMapper'-Objekt basierend auf dem gegebenen Strategienamen.
    /// Wenn kein Strategienamen angegeben wird, wird die Standardstrategie verwendet.
    /// </summary>
    /// <param name="strategyName">Der Name der Strategie. Standardwert ist der Name der Standardstrategie.</param>
    /// <returns>Ein 'Result'-Objekt, das entweder das erstellte 'WorkflowStrategyMapper'-Objekt enthält (im Erfolgsfall) oder einen Fehlerstring (im Fehlerfall).</returns>
    public static Result<WorkflowStrategyMapper, string> Create(string? strategyName = DefaultStrategy)
    {
        if (string.IsNullOrWhiteSpace(strategyName))
        {
            strategyName = DefaultStrategy;
        }
        
        if (StrategyMap.TryGetValue(strategyName, out var strategy))
        {
            return Result.Success<WorkflowStrategyMapper, string>(new WorkflowStrategyMapper(strategy));
        }

        return Result.Failure<WorkflowStrategyMapper, string>($"Unknown strategy: {strategyName}");
    }
}


