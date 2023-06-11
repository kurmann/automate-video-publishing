using TagLib;
using CSharpFunctionalExtensions;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

namespace AutomateVideoPublishing.Entities;

public class Mpeg4MetadataContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public TagLib.Tag[] Tags { get; }

    private Mpeg4MetadataContainer(TagLib.Tag[] tags) => Tags = tags;

    private static Result<IReadOnlyDictionary<string, string?>> TryGetMetadata(MediaFileInfoContainer fileContainer)
    {
        var directories = ImageMetadataReader.ReadMetadata(fileContainer.File.FullName);

        var metadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

        if (metadata != null)
        {
            var metadataDict = metadata.Tags.ToDictionary(tag => tag.Name, tag => tag.Description);
            return Result.Success((IReadOnlyDictionary<string, string?>)metadataDict);
        }

        return Result.Failure<IReadOnlyDictionary<string, string?>>("QuickTime metadata not found");
    }

    public static Result<Mpeg4MetadataContainer> Create(string? filePath)
    {
        try
        {
            var tagLibFile = TagLib.File.Create(filePath);
            var tags = ((TagLib.CombinedTag)((TagLib.Mpeg4.File)tagLibFile).Tag).Tags;

            var combinedTagByTagLib = tagLibFile.Tag;

            var directories = ImageMetadataReader.ReadMetadata(filePath);
            var metadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();
            

            return new Mpeg4MetadataContainer(tags);
        }
        catch (Exception ex)
        {
            return Result.Failure<Mpeg4MetadataContainer>(ex.Message);
        }

        
    }
}