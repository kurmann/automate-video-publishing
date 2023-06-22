using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

namespace AutomateVideoPublishing.Entities;

public class QuickTimeMetadataContainer : MediaFileInfoContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public IReadOnlyDictionary<string, string?> RawMetadata { get; private set; }

    public string? Description => RawMetadata.GetValueOrDefault("Description");

    private QuickTimeMetadataContainer(FileInfo file, MediaType mediaType, IReadOnlyDictionary<string, string?> rawMetadata)
        : base(file, mediaType) => RawMetadata = rawMetadata;

    private static Result<IReadOnlyDictionary<string, string?>> TryGetQuickTimeMetadata(FileInfo file)
    {
        var directories = ImageMetadataReader.ReadMetadata(file.FullName);

        var metadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

        if (metadata != null)
        {
            var metadataDict = metadata.Tags.ToDictionary(tag => tag.Name, tag => tag.Description);
            return Result.Success((IReadOnlyDictionary<string, string?>)metadataDict);
        }

        return Result.Failure<IReadOnlyDictionary<string, string?>>("QuickTime metadata not found");
    }

    public static new Result<QuickTimeMetadataContainer> Create(string? filePath)
    {
        var result = MediaFileInfoContainer.Create(filePath);
        if (result.IsFailure)
        {
            return Result.Failure<QuickTimeMetadataContainer>(result.Error);
        }

        if (result.Value.MediaType != MediaType.QuickTimeMov)
        {
            return Result.Failure<QuickTimeMetadataContainer>($"File {filePath} is not a QuickTime .mov file.");
        }

        var metadataResult = TryGetQuickTimeMetadata(result.Value.FileInfo);
        if (metadataResult.IsFailure)
        {
            return Result.Failure<QuickTimeMetadataContainer>(metadataResult.Error);
        }

        return Result.Success(new QuickTimeMetadataContainer(result.Value.FileInfo, result.Value.MediaType, metadataResult.Value));
    }
}
