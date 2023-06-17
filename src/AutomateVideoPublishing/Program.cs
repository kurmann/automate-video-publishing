using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Strategies;
using CommandLine;
using CSharpFunctionalExtensions;

class Program
{
    private const string DefaultStrategyName = "TransmitMetadata";

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                var contextResult = WorkflowContext.Create(opts.SourceFile, opts.TargetFile);
                if (contextResult.IsFailure)
                {
                    Console.WriteLine($"Error setting up workflow context: {contextResult.Error}");
                    return;
                }

                var strategyMapperResult =  WorkflowStrategyMapper.Create(opts.Strategy);
                if (strategyMapperResult.IsFailure)
                {
                    Console.WriteLine(strategyMapperResult.Error);
                    return;
                }

                strategyMapperResult.Value.SelectedStrategy.Execute(contextResult.Value);
            });
    }

    private static Result ExecuteStrategy(WorkflowContext context, string strategyName, Dictionary<string, IWorkflowStrategy> strategyMap) => strategyMap.ContainsKey(strategyName)
            ? Result.Success(strategyMap[strategyName].Execute(context))
            : Result.Failure($"Unknown strategy: {strategyName}");
}

public class Options
{
    [Option('s', "source", Required = true, HelpText = "Source file.")]
    public string? SourceFile { get; set; }

    [Option('t', "target", Required = true, HelpText = "Target file.")]
    public string? TargetFile { get; set; }

    [Option('y', "strategy", Required = false, HelpText = "Strategy to execute.")]
    public string? Strategy { get; set; }

    public Result ValidateInputParameters()
    {
        if (string.IsNullOrWhiteSpace(SourceFile))
        {
            return Result.Failure("Source file cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(TargetFile))
        {
            return Result.Failure("Target file cannot be empty.");
        }

        return Result.Success();
    }
}

/// <summary>
/// Die Klasse 'WorkflowStrategyMapper' ermöglicht das Erstellen von IWorkflowStrategy-Objekten basierend auf dem Namen einer Strategie.
/// </summary>
public class WorkflowStrategyMapper
{
    // Map der unterstützten Strategien. 
    // Der Schlüssel ist der Name der Strategie und der Wert ist das zugehörige IWorkflowStrategy-Objekt.
    private static readonly Dictionary<string, IWorkflowStrategy> StrategyMap = new()
    {
        { nameof(TransmitMetadataStrategy), new TransmitMetadataStrategy() }
        // Hier können weitere Strategien hinzugefügt werden.
    };

    // Die Standardstrategie, die verwendet wird, wenn keine spezifische Strategie angegeben wird.
    private const string DefaultStrategy = nameof(TransmitMetadataStrategy);

    // Die aktuell gewählte Strategie.
    public IWorkflowStrategy SelectedStrategy { get; private set; }

    private WorkflowStrategyMapper(IWorkflowStrategy strategy)
    {
        SelectedStrategy = strategy;
    }

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
            return Result.Failure<WorkflowStrategyMapper, string>("Strategy parameter cannot be empty or null.");
        }
        
        if (StrategyMap.TryGetValue(strategyName, out var strategy))
        {
            return Result.Success<WorkflowStrategyMapper, string>(new WorkflowStrategyMapper(strategy));
        }

        return Result.Failure<WorkflowStrategyMapper, string>($"Unknown strategy: {strategyName}");
    }
}


