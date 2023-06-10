using MetadataExtractor;
using CSharpFunctionalExtensions;
using System.Collections.ObjectModel;

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
    public List<IReadOnlyDictionary<string, string?>> RawMetadata { get; }

    // Private constructor that can only be called within this class.
    private ReadOnlyMediaMetadataContainer(List<IReadOnlyDictionary<string, string?>> rawMetadata) => RawMetadata = rawMetadata;

    private static Result<List<IReadOnlyDictionary<string, string?>>> TryGetQuickTimeMetadata(MediaFileInfoContainer fileContainer)
    {
        try
        {
            var directories = ImageMetadataReader.ReadMetadata(fileContainer.File.FullName);
            var allTags = new List<IReadOnlyDictionary<string, string?>>();

            foreach (var directory in directories)
            {
                var tagDictionary = new Dictionary<string, string?>(
                    directory.Tags.ToDictionary(tag => tag.Name, tag => tag.Description)
                );
                allTags.Add(new ReadOnlyDictionary<string, string?>(tagDictionary));
            }

            return Result.Success<List<IReadOnlyDictionary<string, string?>>>(allTags);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<IReadOnlyDictionary<string, string?>>>($"Error on reading media metadata: {ex.Message}");
        }
    }

    public static Result<ReadOnlyMediaMetadataContainer> Create(string? filePath) => MediaFileInfoContainer.Create(filePath)
        .Bind(fileInfoContainer => TryGetQuickTimeMetadata(fileInfoContainer)
        .Map(metadata => new ReadOnlyMediaMetadataContainer(metadata)));

    public static Result<ReadOnlyMediaMetadataContainer> Create(MediaFileInfoContainer mediaFileInfoContainer) => TryGetQuickTimeMetadata(mediaFileInfoContainer)
        .Map(metadata => new ReadOnlyMediaMetadataContainer(metadata));
}
