using System.Globalization;
using System.Text.Json;

namespace AutomateVideoPublishing.Entities.Metadata;

public class EssentialMpeg4Metadata
{
    public Maybe<string> Title { get; }
    public Maybe<string> Description { get; }
    public Maybe<string> Album { get; }
    public Maybe<string> HDR_Format { get; }
    public Maybe<int> Width { get; }
    public Maybe<int> Height { get; }
    public Maybe<float> FrameRate { get; }
    public Maybe<string> FileName { get; }
    public Maybe<string> DurationString { get; }
    public Maybe<TimeSpan> Duration => GetDuration(DurationString);
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
        Description = GetValue<string>(jsonObject, "Title_More");
        Album = GetValue<string>(jsonObject, "Album");
        HDR_Format = GetValue<string>(jsonObject, "HDR_Format");
        Width = GetValue<int>(jsonObject, "Width");
        Height = GetValue<int>(jsonObject, "Height");
        FrameRate = GetValue<float>(jsonObject, "FrameRate");
        FileName = GetValue<string>(jsonObject, "FileName");
        DurationString = GetValue<string>(jsonObject, "Duration_String3");
        FileExtension = GetValue<string>(jsonObject, "FileExtension");
        Format = GetValue<string>(jsonObject, "Format");
        Producer = GetValue<string>(jsonObject, "Producer");
        InternetMediaType = GetValue<string>(jsonObject, "InternetMediaType");
        BitRate = GetValue<double>(jsonObject, "BitRate");
        ChromaSubsampling = GetValue<string>(jsonObject, "ChromaSubsampling");
        BitDepth = GetValue<int>(jsonObject, "BitDepth");
        FileSize = GetValue<long>(jsonObject, "FileSize");
        Encoded_Date = GetValue<DateTime>(jsonObject, "Encoded_Date");
        // Extra = GetValue<List<string>>(jsonObject, "extra");
    }

    public static Result<EssentialMpeg4Metadata> Create(JsonDocument? jsonDocument)
    {
        if (jsonDocument == null)
        {
            return Result.Failure<EssentialMpeg4Metadata>("JSON Dokument darf nicht leer sein");
        }

        var jsonElement = jsonDocument.RootElement;
        if (!jsonElement.TryGetProperty("creatingLibrary", out var creatingLibraryProperty)
            || creatingLibraryProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<EssentialMpeg4Metadata>("Das JSON-Objekt enthält nicht das erforderliche 'creatingLibrary' Element.");
        }

        if (!creatingLibraryProperty.TryGetProperty("name", out var nameProperty)
            || nameProperty.GetString() != "MediaInfoLib")
        {
            return Result.Failure<EssentialMpeg4Metadata>("Das 'creatingLibrary' Element enthält nicht den erwarteten 'MediaInfoLib' Namen.");
        }

        if (!jsonElement.TryGetProperty("media", out var mediaProperty)
            || mediaProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<EssentialMpeg4Metadata>("Das JSON-Objekt enthält nicht das erforderliche 'media' Element.");
        }

        if (!mediaProperty.TryGetProperty("track", out var trackProperty)
            || trackProperty.ValueKind != JsonValueKind.Array
            || trackProperty.GetArrayLength() == 0)
        {
            return Result.Failure<EssentialMpeg4Metadata>("Das 'media' Element enthält kein Array 'track' oder das Array ist leer.");
        }

        var generalTrackElement = trackProperty[0];  // Get the first object from the track array

        return Result.Success(new EssentialMpeg4Metadata(generalTrackElement));
    }



    public Result<YamlContent> GetYamlContent() => YamlContent.CreateFromMetadataSections(this);

    private static Maybe<T> GetValue<T>(JsonElement jsonObject, string propertyName)
    {
        if (jsonObject.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind != JsonValueKind.Null)
            {
                if (typeof(T) == typeof(DateTime))
                {
                    var dateString = JsonSerializer.Deserialize<string>(property.GetRawText());
                    return (T)(object)ParseDateTime(dateString);
                }
                else
                {
                    var options = new JsonSerializerOptions
                    {
                        NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                    };
                    var value = JsonSerializer.Deserialize<T>(property.GetRawText(), options);
                    return value != null ? value : Maybe<T>.None;
                }
            }
        }

        return Maybe<T>.None;
    }

    private static DateTime ParseDateTime(string? dateString)
    {
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
        {
            return result;
        }

        throw new FormatException($"Cannot parse '{dateString}' as a valid DateTime.");
    }



    private static Maybe<TimeSpan> GetDuration(Maybe<string> durationString)
    {
        if (durationString.HasNoValue)
        {
            return Maybe<TimeSpan>.None;
        }
        if (TimeSpan.TryParseExact(durationString.Value, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, out var duration))
        {
            return Maybe<TimeSpan>.From(duration);
        }

        return Maybe<TimeSpan>.None;
    }

}
