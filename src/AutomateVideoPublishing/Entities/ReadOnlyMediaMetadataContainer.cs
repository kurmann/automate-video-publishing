using MetadataExtractor;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Manages the metadata of a media file.
/// The metadata is read and stored upon instance creation.
/// </summary>
public class ReadOnlyMediaMetadataContainer
{
    /// <summary>
    /// Contains the raw metadata read from the file.
    /// </summary>
    public IReadOnlyList<MetadataExtractor.Directory> RawMetadata { get; }

    // Private constructor that can only be called within this class.
    private ReadOnlyMediaMetadataContainer(IReadOnlyList<MetadataExtractor.Directory> rawMetadata) => RawMetadata = rawMetadata;

    private static Result<IReadOnlyList<MetadataExtractor.Directory>> TryGetQuickTimeMetadata(MediaFileInfoContainer fileContainer)
    {
        try
        {
            var directory = ImageMetadataReader.ReadMetadata(fileContainer.File.FullName);
            return Result.Success<IReadOnlyList<MetadataExtractor.Directory>>(directory);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<MetadataExtractor.Directory>>($"Error on reading media metadata: {ex.Message}");
        }
    }

    public static Result<ReadOnlyMediaMetadataContainer> Create(string? filePath) => MediaFileInfoContainer.Create(filePath)
        .Bind(fileInfoContainer => TryGetQuickTimeMetadata(fileInfoContainer)
        .Map(metadata => new ReadOnlyMediaMetadataContainer(metadata)));

    public static Result<ReadOnlyMediaMetadataContainer> Create(MediaFileInfoContainer mediaFileInfoContainer) => TryGetQuickTimeMetadata(mediaFileInfoContainer)
        .Map(metadata => new ReadOnlyMediaMetadataContainer(metadata));
}
