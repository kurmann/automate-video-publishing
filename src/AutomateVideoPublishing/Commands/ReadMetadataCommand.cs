using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Commands;

public class ReadMetadataCommand : IAsyncCommand<string, Result>
{
    private readonly ILogger _logger;
    private readonly MediaInfoManager _manager;

    public ReadMetadataCommand(MediaInfoManager mediaInfoManager)
    {
        _logger = LogManager.GetCurrentClassLogger();
        _manager = mediaInfoManager;
    }

    public async Task<Result> ExecuteAsync(string filePath)
    {
        var supportedMediaFileResult = SupportedMediaFile.Create(filePath);
        if (supportedMediaFileResult.IsFailure)
        {
            return Result.Failure(supportedMediaFileResult.Error);
        }

        var jsonDocResult = await _manager.RunAsync(filePath);
        if (jsonDocResult.IsFailure)
        {
            return Result.Failure($"Error on reading JSON data using MediaInfo CLI: {jsonDocResult.Error}");
        }

        Result<YamlContent> yamlContentResult;
        switch (supportedMediaFileResult.Value.MediaType)
        {
            case SupportedMediaType.Mpeg4:
                var mpeg4Metadata = EssentialMpeg4Metadata.Create(jsonDocResult.Value);
                if (mpeg4Metadata.IsFailure)
                {
                    return Result.Failure(mpeg4Metadata.Error);
                }
                yamlContentResult = mpeg4Metadata.Value.GetYamlContent();
                break;
            case SupportedMediaType.QuickTime:
                var quickTimeMetadata = EssentialQuickTimeMetadata.Create(jsonDocResult.Value);
                if (quickTimeMetadata.IsFailure)
                {
                    return Result.Failure(quickTimeMetadata.Error);
                }
                yamlContentResult = quickTimeMetadata.Value.GetYamlContent();
                break;
            default:
                return Result.Failure("Unsupported media type.");
        }

        if (yamlContentResult.IsFailure)
        {
            return Result.Failure($"Error on creating YAMl content from essential data: {yamlContentResult.Error}");
        }

        var yamlWriteResult = await WriteYamlToFileAsync(yamlContentResult.Value, filePath);
        if (yamlWriteResult.IsFailure)
        {
            return Result.Failure($"Error on writing YAML content to file: {yamlWriteResult.Error}");
        }

        return yamlWriteResult;
    }

    private async Task<Result> WriteYamlToFileAsync(YamlContent yamlData, string filePath)
    {
        var yamlFilePath = Path.ChangeExtension(filePath, "yaml");
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
