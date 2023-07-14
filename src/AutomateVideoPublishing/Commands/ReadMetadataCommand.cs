using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Liest alle Metadaten aus der QuickTime-Datei und speichert sie als gleichnamiges YAML.
/// </summary>
public class ReadMetadataCommand : IAsyncCommand<string, Result>
{
    private readonly ILogger _logger;
    private readonly MediaInfoManager _manager;

    public ReadMetadataCommand(MediaInfoManager mediaInfoManager)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _manager = mediaInfoManager;
    }

    public async Task<Result> ExecuteAsync(string masterfilePath)
    {
        var jsonDoc = await _manager.RunAsync(masterfilePath);
        if (jsonDoc.IsFailure)
        {
            return Result.Failure($"Error on reading JSON data using MediaInfo CLI: {jsonDoc.Error}");
        }

        var essentialMpeg4Metadate = EssentialMpeg4Metadata.Create(jsonDoc.Value);
        if (essentialMpeg4Metadate.IsFailure)
        {
            return Result.Failure(essentialMpeg4Metadate.Error);
        }
        var yamlContent = essentialMpeg4Metadate.Value.GetYamlContent();
        if (yamlContent.IsFailure)
        {
            return Result.Failure($"Error on creating YAMl content from essential MPEG-4 Data: {yamlContent.Value}");
        }
        var yamlWriteResult = await WriteYamlToFileAsync(yamlContent.Value, masterfilePath);
        if (yamlWriteResult.IsFailure)
        {
            return Result.Failure($"Error on writing YAML content to file: {yamlWriteResult.Error}");
        }

        return yamlWriteResult;
    }

    private async Task<Result> WriteYamlToFileAsync(YamlContent yamlData, string masterfilePath)
    {
        var yamlFilePath = Path.ChangeExtension(masterfilePath, "yaml");
        try
        {
            await File.WriteAllTextAsync(yamlFilePath, yamlData.Value);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to write YAML content to file: {ex.Message}");
        }
    }
}
