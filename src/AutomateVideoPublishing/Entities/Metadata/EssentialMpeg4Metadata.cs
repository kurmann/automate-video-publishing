using System.Text.Json;

namespace AutomateVideoPublishing.Entities.Metadata;

public class EssentialMpeg4Metadata
{
    public Maybe<string> Title { get; }
    public Maybe<string> Album { get; }
    public Maybe<string> HDR_Format { get; }
    public Maybe<int> Width { get; }
    public Maybe<int> Height { get; }
    public Maybe<float> FrameRate { get; }
    public Maybe<string> FileName { get; }
    public Maybe<string> Duration_String3 { get; }
    public Maybe<string> FileExtension { get; }
    public Maybe<string> Format { get; }
    public Maybe<string> Producer { get; }
    public Maybe<string> InternetMediaType { get; }
    public Maybe<double> BitRate { get; }
    public Maybe<string> ChromaSubsampling { get; }
    public Maybe<int> BitDepth { get; }
    public Maybe<long> FileSize { get; }
    public Maybe<DateTime> Encoded_Date { get; }
    public Maybe<List<string>> Extra { get; }

    private EssentialMpeg4Metadata(JsonElement jsonObject)
    {
        Title = GetValue<string>(jsonObject, "Title");
        Album = GetValue<string>(jsonObject, "Album");
        HDR_Format = GetValue<string>(jsonObject, "HDR_Format");
        Width = GetValue<int>(jsonObject, "Width");
        Height = GetValue<int>(jsonObject, "Height");
        FrameRate = GetValue<float>(jsonObject, "FrameRate");
        FileName = GetValue<string>(jsonObject, "FileName");
        Duration_String3 = GetValue<string>(jsonObject, "Duration_String3");
        FileExtension = GetValue<string>(jsonObject, "FileExtension");
        Format = GetValue<string>(jsonObject, "Format");
        Producer = GetValue<string>(jsonObject, "Producer");
        InternetMediaType = GetValue<string>(jsonObject, "InternetMediaType");
        BitRate = GetValue<double>(jsonObject, "BitRate");
        ChromaSubsampling = GetValue<string>(jsonObject, "ChromaSubsampling");
        BitDepth = GetValue<int>(jsonObject, "BitDepth");
        FileSize = GetValue<long>(jsonObject, "FileSize");
        Encoded_Date = GetValue<DateTime>(jsonObject, "Encoded_Date");
        Extra = GetValue<List<string>>(jsonObject, "extra");
    }

    public static Result<EssentialMpeg4Metadata> Create(JsonElement jsonObject)
    {
        if (!jsonObject.TryGetProperty("creatingLibrary", out var creatingLibraryProperty)
            || creatingLibraryProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<EssentialMpeg4Metadata>("Das JSON-Objekt enthält nicht das erforderliche 'creatingLibrary' Element.");
        }

        if (!creatingLibraryProperty.TryGetProperty("name", out var nameProperty)
            || nameProperty.GetString() != "MediaInfoLib")
        {
            return Result.Failure<EssentialMpeg4Metadata>("Das 'creatingLibrary' Element enthält nicht den erwarteten 'MediaInfoLib' Namen.");
        }

        return Result.Success(new EssentialMpeg4Metadata(jsonObject));
    }

    private static Maybe<T> GetValue<T>(JsonElement jsonObject, string propertyName)
    {
        if (jsonObject.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind != JsonValueKind.Null)
            {
                var value = JsonSerializer.Deserialize<T>(property.GetRawText());
                return value != null ? value : Maybe<T>.None;
            }
        }

        return Maybe<T>.None;
    }
}
