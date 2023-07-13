using System.Globalization;
using System.Text.Json;

namespace AutomateVideoPublishing.Entities.Metadata;

public class EssentialMpeg4Metadata
{
    public Maybe<string> Title { get; }
    public Maybe<string> Description { get; }
    public Maybe<string> Album { get; }
    public Maybe<string> HdrFormat { get; }
    public Maybe<string> WidthString { get; }
    public Maybe<string> HeightString { get; }
    public Maybe<string> FrameRateString { get; }
    public Maybe<string> DurationString { get; }
    public Maybe<TimeSpan> Duration => GetDuration(DurationString);
    public Maybe<string> Format { get; }
    public Maybe<string> Producer { get; }
    public Maybe<string> InternetMediaType { get; }
    public Maybe<string> BitRateString { get; }
    public Maybe<string> ChromaSubsampling { get; }
    public Maybe<string> BitDepth { get; }
    public Maybe<string> FileSize { get; }
    public Maybe<string> EncodedDateString { get; }
    public Maybe<string> Extra { get; }

    private EssentialMpeg4Metadata(JsonElement jsonObject)
    {
        Title = GetValue(jsonObject, "Title");
        Description = GetValue(jsonObject, "Title_More");
        Album = GetValue(jsonObject, "Album");
        HdrFormat = GetValue(jsonObject, "HDR_Format");
        WidthString = GetValue(jsonObject, "Width");
        HeightString = GetValue(jsonObject, "Height");
        FrameRateString = GetValue(jsonObject, "FrameRate");
        DurationString = GetValue(jsonObject, "Duration_String3");
        Format = GetValue(jsonObject, "Format");
        Producer = GetValue(jsonObject, "Producer");
        InternetMediaType = GetValue(jsonObject, "InternetMediaType");
        BitRateString = GetValue(jsonObject, "BitRate");
        ChromaSubsampling = GetValue(jsonObject, "ChromaSubsampling");
        BitDepth = GetValue(jsonObject, "BitDepth");
        FileSize = GetValue(jsonObject, "FileSize");
        EncodedDateString = GetValue(jsonObject, "Encoded_Date");
        // Extra = GetValue(jsonObject, "extra");
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

    private static Maybe<string> GetValue(JsonElement jsonObject, string propertyName)
    {
        if (jsonObject.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind != JsonValueKind.Null)
            {
                var value = property.GetString();
                return value != null ? Maybe<string>.From(value) : Maybe<string>.None;
            }
        }

        return Maybe<string>.None;
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
