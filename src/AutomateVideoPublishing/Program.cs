using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Strategies;
using CommandLine;
using CSharpFunctionalExtensions;

class Program
{
    static void Main(string[] args)
    {
        // Kommandozeilenoptionen parsen
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                // Workflow-Kontext erstellen
                var contextResult = WorkflowContext.Create(opts.QuickTimeMasterDirectory, opts.PublishedMpeg4Directory);
                if (contextResult.IsFailure)
                {
                    Console.WriteLine($"Error setting up workflow context: {contextResult.Error}");
                    return;
                }

                // Strategie mappen
                var strategyMapperResult =  WorkflowStrategyMapper.Create(opts.Workflow);
                if (strategyMapperResult.IsFailure)
                {
                    Console.WriteLine(strategyMapperResult.Error);
                    return;
                }

                // Ausgewählte Strategie ausführen
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

    [Option('w', "workflow", Required = false, HelpText = "Workflow strategy to execute.")]
    public string? Workflow { get; set; }
}
