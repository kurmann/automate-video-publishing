using TagLib;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

public class Mpeg4MetadataContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public Tag[] Tags { get; }

    private Mpeg4MetadataContainer(Tag[] tags) => Tags = tags;

    public static Result<Mpeg4MetadataContainer> Create(string? filePath)
    {
        try
        {
            var tagLibFile = TagLib.File.Create(filePath);
            var tags = ((TagLib.CombinedTag)((TagLib.Mpeg4.File)tagLibFile).Tag).Tags;
            return new Mpeg4MetadataContainer(tags);
        }
        catch (Exception ex)
        {
            return Result.Failure<Mpeg4MetadataContainer>(ex.Message);
        }

        
    }
}