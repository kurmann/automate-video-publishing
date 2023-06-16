using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Strategies;
using CommandLine;
using CSharpFunctionalExtensions;

class Program
{
    static void Main(string[] args)
    {
        var strategyMap = new Dictionary<string, IWorkflowStrategy>
        {
            { "TransmitMetadata", new TransmitMetadataStrategy() }
            // Weitere Strategien können hier hinzugefügt werden.
        };

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                opts.ValidateInputParameters()
                    .Map(() => WorkflowContext.Create(opts.SourceFile, opts.TargetFile)
                    .Map((context) => ExecuteStrategy(context, opts.Strategy ?? "TransmitMetadata", strategyMap))
                    .Tap(result => Console.WriteLine(result.IsSuccess ? "Workflow completed" : result.Error)));
            });
    }

    private static Result ExecuteStrategy(WorkflowContext context, string strategyName, Dictionary<string, IWorkflowStrategy> strategyMap)
    {
        return strategyMap.ContainsKey(strategyName)
            ? Result.Success(strategyMap[strategyName].Execute(context))
            : Result.Failure($"Unknown strategy: {strategyName}");
    }
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
