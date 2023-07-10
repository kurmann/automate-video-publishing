using System.Text;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies;

/// <summary>
/// Strategie für das Publizieren einer einzelnen komprimierten Videodatei, die aus der Masterdatei erstellt wird.
/// </summary>
public class SingleVideoPublishStrategy : IAsyncWorkflow
{
    private readonly Logger _logger;

    public SingleVideoPublishStrategy() => _logger = LogManager.GetCurrentClassLogger();

    public async Task ExecuteAsync(WorkflowContext context)
    {
        _logger.Info("Start executing SingleVideoPublishStrategy");

        // Bereite die Commands vor
        var readMasterfileMetadataCommand = new ReadMasterfileMetadataCommand();

        // Führe die Strategie für jede QuickTime-Datei aus.
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Using {context.QuickTimeMasterDirectory.Directory.FullName} as directory for the QuickTime Masterfiles");
        stringBuilder.AppendJoin(", ", context.QuickTimeMasterDirectory.QuickTimeFiles.Select(file => file.Name));
        foreach (var masterfile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            var commandResult = await readMasterfileMetadataCommand.ExecuteAsync(masterfile.FullName);
            if (commandResult.IsFailure)
            {
                _logger.Error(commandResult.Error);
                continue;
            }
        }

        _logger.Info("Finished executing SingleVideoPublishStrategy");
    }
}
