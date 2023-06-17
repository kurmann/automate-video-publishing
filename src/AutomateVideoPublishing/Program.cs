using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Strategies;
using CommandLine;
using CSharpFunctionalExtensions;

class Program
{
    private const string DefaultStrategyName = "TransmitMetadata";
    private const string DefaultDirectory = ".";  // Current directory

    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                // use default values if not provided
                var quickTimeMasterDirectory = string.IsNullOrWhiteSpace(opts.QuickTimeMasterDirectory) ? DefaultDirectory : opts.QuickTimeMasterDirectory;
                var publishedMpeg4Directory = string.IsNullOrWhiteSpace(opts.PublishedMpeg4Directory) ? DefaultDirectory : opts.PublishedMpeg4Directory;
                var strategy = string.IsNullOrWhiteSpace(opts.Workflow) ? DefaultStrategyName : opts.Workflow;

                var contextResult = WorkflowContext.Create(quickTimeMasterDirectory, publishedMpeg4Directory);
                if (contextResult.IsFailure)
                {
                    Console.WriteLine($"Error setting up workflow context: {contextResult.Error}");
                    return;
                }

                var strategyMapperResult =  WorkflowStrategyMapper.Create(strategy);
                if (strategyMapperResult.IsFailure)
                {
                    Console.WriteLine(strategyMapperResult.Error);
                    return;
                }

                strategyMapperResult.Value.SelectedStrategy.Execute(contextResult.Value);
            });
    }
}

public class Options
{
    [Option('s', "source", Required = false, HelpText = "Source file.")]
    public string? QuickTimeMasterDirectory { get; set; }

    [Option('t', "target", Required = false, HelpText = "Target file.")]
    public string? PublishedMpeg4Directory { get; set; }

    [Option('w', "workflow", Required = false, HelpText = "Strategy to execute.")]
    public string? Workflow { get; set; }
}
