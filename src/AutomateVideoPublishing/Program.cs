using Microsoft.Extensions.Configuration;

class Program
{
    static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .AddCommandLine(args)
            .Build();

        var options = new Options();
        configuration.Bind(options);

        Console.WriteLine($"Workflow param: {options.Workflow}");

        // Workflow-Kontext erstellen
        var contextResult = WorkflowContext.Create(options.QuickTimeMasterDirectory, options.PublishedMpeg4Directory);
        if (contextResult.IsFailure)
        {
            Console.WriteLine($"Error setting up workflow context: {contextResult.Error}");
            return;
        }

        // Strategie mappen und ausführen
        var workflowResult = WorkflowStrategyMapper.Create(options.Workflow)
            .Map(strategyMapper => strategyMapper.SelectedStrategy.Execute(contextResult.Value));
        Console.WriteLine(workflowResult.IsFailure ? workflowResult.Error : "Workflow completed");
    }
}


public class Options
{
    public string? QuickTimeMasterDirectory { get; set; }

    public string? PublishedMpeg4Directory { get; set; }

    public string? Workflow { get; set; }
}
