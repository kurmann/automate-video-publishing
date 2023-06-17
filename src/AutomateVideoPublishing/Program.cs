using CommandLine;

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

                // Strategie mappen und ausführen
                var workflowResult = WorkflowStrategyMapper.Create(opts.Workflow)
                    .Map(strategyMapper => strategyMapper.SelectedStrategy.Execute(contextResult.Value));
;
                Console.WriteLine(workflowResult.IsFailure ? workflowResult.Error : "Workflow completed");
            });
    }
}

public class Options
{
    [Option('s', "source", Required = false, HelpText = "Optional. Source directory for QuickTime master videos. If not specified, the current working directory is used.")]
    public string? QuickTimeMasterDirectory { get; set; }

    [Option('t', "target", Required = false, HelpText = "Optional. Target directory for published MPEG-4 videos. If not specified, the current working directory is used.")]
    public string? PublishedMpeg4Directory { get; set; }

    [Option('w', "workflow", Required = false, Default = WorkflowStrategyMapper.DefaultStrategy, HelpText = "Specifies the workflow strategy to execute.")]
    public string? Workflow { get; set; }
}
