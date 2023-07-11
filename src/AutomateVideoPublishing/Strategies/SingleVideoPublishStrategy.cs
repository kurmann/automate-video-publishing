using System.Text;
using AutomateVideoPublishing.Commands;
using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Strategies;

/// <summary>
/// Strategie für das Publizieren einer einzelnen komprimierten Videodatei, die aus der Masterdatei erstellt wird.
/// </summary>
public class SingleVideoPublishStrategy : IAsyncWorkflow
{
    private readonly Logger _logger;

    public SingleVideoPublishStrategy() => _logger = LogManager.GetCurrentClassLogger();

    public async Task<Result> ExecuteAsync(WorkflowContext context)
    {
        _logger.Info("Start executing SingleVideoPublishStrategy");

        // Bereite die Commands vor
        var readMasterfileMetadataCommand = new ReadMasterfileMetadataCommand(new MediaInfoManager());
        string outputDir = context.PublishedMpeg4Directory.Directory.FullName;
        var compressMasterfileCommand = new CompressMasterfileCommand(new AppleCompressorManager(), outputDir);

        // Führe die Strategie für jede QuickTime-Datei aus.
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Using {context.QuickTimeMasterDirectory.Directory.FullName} as directory for the QuickTime Masterfiles");
        stringBuilder.AppendJoin(", ", context.QuickTimeMasterDirectory.QuickTimeFiles.Select(file => file.Name));
        foreach (var masterfile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            var readMetadataResult = await readMasterfileMetadataCommand.ExecuteAsync(masterfile.FullName);
            if (readMetadataResult.IsFailure)
            {
                _logger.Error(readMetadataResult.Error);
                continue;
            }

            var compressResult = await compressMasterfileCommand.ExecuteAsync(masterfile.FullName);
            if (compressResult.IsFailure)
            {
                _logger.Error(compressResult.Error);
                continue;
            }
        }

        _logger.Info("Finished executing SingleVideoPublishStrategy");
        return Result.Success();
    }
}
