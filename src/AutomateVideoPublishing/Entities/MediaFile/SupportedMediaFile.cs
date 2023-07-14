namespace AutomateVideoPublishing.Entities.MediaFile;

public enum SupportedMediaType
{
    Mpeg4,
    QuickTime
}

/// <summary>
/// Represents a supported media file, which can be either a MPEG4 or QuickTime file.
/// </summary>
public class SupportedMediaFile
{
    public FileInfo Value { get; }
    public SupportedMediaType MediaType { get; }
    public Mpeg4File Mpeg4File { get; }
    public QuickTimeMasterFile QuickTimeFile { get; }

    private SupportedMediaFile(FileInfo value, SupportedMediaType mediaType, Mpeg4File mpeg4File = null, QuickTimeMasterFile quickTimeFile = null)
    {
        Value = value;
        MediaType = mediaType;
        Mpeg4File = mpeg4File;
        QuickTimeFile = quickTimeFile;
    }

    public static Result<SupportedMediaFile> Create(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Failure<SupportedMediaFile>("File path cannot be null or whitespace.");
        }

        var file = new FileInfo(filePath);
        if (!file.Exists)
        {
            return Result.Failure<SupportedMediaFile>("File does not exist.");
        }

        var mpeg4Result = Mpeg4File.Create(filePath);
        if (mpeg4Result.IsSuccess)
        {
            return Result.Success(new SupportedMediaFile(file, SupportedMediaType.Mpeg4, mpeg4File: mpeg4Result.Value));
        }

        var quickTimeResult = QuickTimeMasterFile.Create(filePath);
        if (quickTimeResult.IsSuccess)
        {
            return Result.Success(new SupportedMediaFile(file, SupportedMediaType.QuickTime, quickTimeFile: quickTimeResult.Value));
        }

        return Result.Failure<SupportedMediaFile>("File type is not supported. Only MPEG4 and QuickTime files are supported.");
    }

}
