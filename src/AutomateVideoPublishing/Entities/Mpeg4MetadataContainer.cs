using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

public class Mpeg4MetadataContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public TagLib.Tag Tags { get; }

    private Mpeg4MetadataContainer(TagLib.Tag tags) => Tags = tags;

    private static Result<TagLib.Tag> TryGetMetadata(MediaFileInfoContainer fileContainer)
    {
        try
        {
            var tagLibFile = TagLib.File.Create(fileContainer.File.FullName);
            if (tagLibFile == null)
            {
                return Result.Failure<TagLib.Tag>($"File parsing of file ´{fileContainer.File.FullName}´ by TagLib returned null.");
            }
            var combinedTags = tagLibFile.Tag;
            if (combinedTags == null)
            {
                return Result.Failure<TagLib.Tag>($"Combined tags of file ´{fileContainer.File.FullName}´ by TagLib is null.");
            }
            return Result.Success<TagLib.Tag>(combinedTags);
        }
        catch (Exception ex)
        {
            return Result.Failure<TagLib.Tag>($"Error on reading metadata using Tag Lib: {ex.Message}");
        }
    }

    public static Result<Mpeg4MetadataContainer> Create(string? filePath) => MediaFileInfoContainer.Create(filePath)
        .Bind(fileInfoContainer => TryGetMetadata(fileInfoContainer)
        .Map(metadata => new Mpeg4MetadataContainer(metadata)));

    public static Result<Mpeg4MetadataContainer> Create(MediaFileInfoContainer mediaFileInfoContainer) => TryGetMetadata(mediaFileInfoContainer)
        .Map(metadata => new Mpeg4MetadataContainer(metadata));
}