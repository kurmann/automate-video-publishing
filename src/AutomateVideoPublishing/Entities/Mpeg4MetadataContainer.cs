namespace AutomateVideoPublishing.Entities;

public class Mpeg4MetadataContainer : MediaFileInfoContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public TagLib.Tag Tags { get; private set; }

    private Mpeg4MetadataContainer(FileInfo file, MediaType mediaType, TagLib.Tag tags) : base(file, mediaType)
    {
        Tags = tags;
    }

    private static Result<TagLib.Tag> TryGetMetadata(FileInfo file)
    {
        try
        {
            var tagLibFile = TagLib.File.Create(file.FullName);
            if (tagLibFile == null)
            {
                return Result.Failure<TagLib.Tag>($"File parsing of file ´{file.FullName}´ by TagLib returned null.");
            }
            var combinedTags = tagLibFile.Tag;
            if (combinedTags == null)
            {
                return Result.Failure<TagLib.Tag>($"Combined tags of file ´{file.FullName}´ by TagLib is null.");
            }
            return Result.Success<TagLib.Tag>(combinedTags);
        }
        catch (Exception ex)
        {
            return Result.Failure<TagLib.Tag>($"Error on reading metadata using Tag Lib: {ex.Message}");
        }
    }

    public static new Result<Mpeg4MetadataContainer> Create(string? filePath)
    {
        var result = MediaFileInfoContainer.Create(filePath);
        if (result.IsFailure)
        {
            return Result.Failure<Mpeg4MetadataContainer>(result.Error);
        }

        if (result.Value.MediaType != MediaType.Mpeg4)
        {
            return Result.Failure<Mpeg4MetadataContainer>($"File {filePath} is not a MPEG-4 .m4v or .mp4 file.");
        }

        var metadataResult = TryGetMetadata(result.Value.FileInfo);
        if (metadataResult.IsFailure)
        {
            return Result.Failure<Mpeg4MetadataContainer>(metadataResult.Error);
        }

        return Result.Success(new Mpeg4MetadataContainer(result.Value.FileInfo, result.Value.MediaType, metadataResult.Value));
    }
}

