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

