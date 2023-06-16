using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Strategies;
using CommandLine;
using CSharpFunctionalExtensions;

class Program
{
    private const string DefaultStrategyName = "TransmitMetadata";

    static void Main(string[] args)
    {
        var strategyFactory = new StrategyFactory();

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                var contextResult = WorkflowContext.Create(opts.SourceFile, opts.TargetFile);
                if (contextResult.IsFailure)
                {
                    Console.WriteLine($"Error setting up workflow context: {contextResult.Error}");
                    return;
                }

                var strategyResult = strategyFactory.GetStrategy(opts.Strategy);
                if (strategyResult.IsFailure)
                {
                    Console.WriteLine(strategyResult.Error);
                    return;
                }

                strategyResult.Value.Execute(contextResult.Value);
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

public class StrategyFactory
{
    private readonly Dictionary<string, IWorkflowStrategy> _strategyMap;

    public StrategyFactory()
    {
        _strategyMap = new Dictionary<string, IWorkflowStrategy>
        {
            { "TransmitMetadata", new TransmitMetadataStrategy() }
            // Hier können Sie weitere Strategien hinzufügen.
        };
    }

    public Result<IWorkflowStrategy, string> GetStrategy(string? strategyName)
    {
        if (string.IsNullOrWhiteSpace(strategyName))
        {
            return Result.Success<IWorkflowStrategy, string>(_strategyMap["TransmitMetadata"]); // Default-Strategie
        }

        if (_strategyMap.TryGetValue(strategyName, out var strategy))
        {
            return Result.Success<IWorkflowStrategy, string>(strategy);
        }

        return Result.Failure<IWorkflowStrategy, string>($"Unknown strategy: {strategyName}");
    }
}

