using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

public class QuickTimeMediaFileContainer : MediaFileInfoContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public IReadOnlyDictionary<string, string?> RawMetadata { get; private set; }

    private QuickTimeMediaFileContainer(FileInfo file, MediaType mediaType, IReadOnlyDictionary<string, string?> rawMetadata)
        : base(file, mediaType)
    {
        RawMetadata = rawMetadata;
    }

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

    public static new Result<QuickTimeMediaFileContainer> Create(string? filePath)
    {
        var result = MediaFileInfoContainer.Create(filePath);
        if (result.IsFailure)
        {
            return Result.Failure<QuickTimeMediaFileContainer>(result.Error);
        }

        if (result.Value.MediaType != MediaType.QuickTimeMov)
        {
            return Result.Failure<QuickTimeMediaFileContainer>($"File {filePath} is not a QuickTime .mov file.");
        }

        var metadataResult = TryGetQuickTimeMetadata(result.Value.File);
        if (metadataResult.IsFailure)
        {
            return Result.Failure<QuickTimeMediaFileContainer>(metadataResult.Error);
        }

        return Result.Success(new QuickTimeMediaFileContainer(result.Value.File, result.Value.MediaType, metadataResult.Value));
    }
}
