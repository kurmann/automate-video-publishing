using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Strategies;
using CommandLine;

class Program
{
    static void Main(string[] args)
    {
        var strategyMap = new Dictionary<string, IWorkflowStrategy>
        {
            { "TransmitMetadata", new TransmitMetadataStrategy() }
            // Hier können Sie weitere Strategien hinzufügen.
        };

        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts =>
            {
                var contextResult = WorkflowContext.Create(opts.SourceFile, opts.TargetFile);
                if (contextResult.IsFailure)
                {
                    Console.WriteLine($"Error setting up workflow context: {contextResult.Error}");
                    return;
                }

                if (string.IsNullOrWhiteSpace(opts.Strategy))
                {
                    Console.WriteLine("Strategy parameter cannot be empty.");
                    return;
                }
                if (strategyMap.TryGetValue(opts.Strategy, out var strategy))
                {
                    strategy.Execute(contextResult.Value);
                }
                else
                {
                    Console.WriteLine("Unknown strategy: {0}", opts.Strategy);
                }
            });
    }
}

public class Options
{
    [Option('s', "source", Required = true, HelpText = "Source file.")]
    public string? SourceFile { get; set; }

    [Option('t', "target", Required = true, HelpText = "Target file.")]
    public string? TargetFile { get; set; }

    [Option('y', "strategy", Required = true, HelpText = "Strategy to execute.")]
    public string? Strategy { get; set; }
}
