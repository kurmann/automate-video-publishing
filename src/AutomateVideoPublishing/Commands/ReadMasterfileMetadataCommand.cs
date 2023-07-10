using System.IO;
using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Liest alle Metadaten aus der QuickTime-Datei und speichert sie als gleichnamiges YAML.
/// </summary>
public class ReadMasterfileMetadataCommand : IAsyncCommand<string, Result>
{
    private readonly ILogger _logger;
    private readonly MediaInfoManager _manager;

    public ReadMasterfileMetadataCommand(MediaInfoManager mediaInfoManager)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _manager = mediaInfoManager;
    }

    public async Task<Result> ExecuteAsync(string masterfilePath)
    {
        var lines = await _manager.RunAsync(masterfilePath);
        var yamlContent = MediaInfoMetadataLineOutput.Create(lines)
            .Map(metadataLineOutput => ParsedQuicktimeMetadata.Create(metadataLineOutput)
            .Bind(parsedQuickTimeMetadata => MediaInfoMetadataYaml.CreateFromMetadataSections(parsedQuickTimeMetadata.RawMetadata)));
        if (yamlContent.IsFailure)
        {
            return Result.Failure(yamlContent.Error);
        }
        if (yamlContent.Value.IsFailure)
        {
            return Result.Failure(yamlContent.Value.Error);
        }
        var result = await WriteYamlToFileAsync(yamlContent.Value.Value, masterfilePath);
        return result;
    }

    private async Task<Result> WriteYamlToFileAsync(MediaInfoMetadataYaml yamlData, string masterfilePath)
    {
        var yamlFilePath = Path.ChangeExtension(masterfilePath, "yaml");
        try
        {
            await File.WriteAllTextAsync(yamlFilePath, yamlData.YamlContent);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to write YAML content to file: {ex.Message}");
        }
    }
}
