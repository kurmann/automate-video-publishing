using System.Text;
using AutomateVideoPublishing.Commands;
using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Workflows;

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
        var readMetadataCommand = new ReadMetadataCommand(new MediaInfoManager());

        // Lese Metadaten für jede Quicktime aus und schreibe es in ein YAML-File
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"Using {context.QuickTimeMasterDirectory.Directory.FullName} as directory for the QuickTime Masterfiles");
        stringBuilder.AppendJoin(", ", context.QuickTimeMasterDirectory.QuickTimeFiles.Select(file => file.Name));
        foreach (var masterfile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            var readMetadataResult = await readMetadataCommand.ExecuteAsync(masterfile.FullName);
            if (readMetadataResult.IsFailure)
            {
                _logger.Error(readMetadataResult.Error);
                continue;
            }
        }

        // Lese Metadaten für jede MPEG-4 aus und schreibe es in ein YAML-File
        stringBuilder = new StringBuilder();
        stringBuilder.Append($"Using {context.PublishedMpeg4Directory.Directory.FullName} as directory for the compressed MPEG-4 files");
        stringBuilder.AppendJoin(", ", context.PublishedMpeg4Directory.Mpeg4Files.Select(file => file.Name));
        foreach (var masterfile in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var readMetadataResult = await readMetadataCommand.ExecuteAsync(masterfile.FullName);
            if (readMetadataResult.IsFailure)
            {
                _logger.Error(readMetadataResult.Error);
                continue;
            }
        }

        _logger.Info("Finished executing SingleVideoPublishStrategy");
        return Result.Success();
    }
}
