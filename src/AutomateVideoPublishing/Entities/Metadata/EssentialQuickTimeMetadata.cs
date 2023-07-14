using System.Globalization;
using System.Text.Json;

namespace AutomateVideoPublishing.Entities.Metadata;

public class EssentialQuickTimeMetadata
{
    public string? Title { get; }
    public string? Description { get; }
    public string? Album { get; }
    public string? HdrFormat { get; }
    public string? WidthString { get; }
    public string? HeightString { get; }
    public string? FrameRateString { get; }
    public string? DurationString { get; }
    public TimeSpan? Duration => GetDuration(DurationString);
    public string? Format { get; }
    public string? Producer { get; }
    public string? InternetMediaType { get; }
    public string? BitRateString { get; }
    public string? ChromaSubsampling { get; }
    public string? BitDepth { get; }
    public string? FileSize { get; }
    public string? EncodedDateString { get; }
    public string? Extra { get; }

    private EssentialQuickTimeMetadata(JsonElement jsonObject)
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

    public static Result<EssentialQuickTimeMetadata> Create(JsonDocument? jsonDocument)
    {
        if (jsonDocument == null)
        {
            return Result.Failure<EssentialQuickTimeMetadata>("JSON Dokument darf nicht leer sein");
        }

        var jsonElement = jsonDocument.RootElement;
        if (!jsonElement.TryGetProperty("creatingLibrary", out var creatingLibraryProperty)
            || creatingLibraryProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<EssentialQuickTimeMetadata>("Das JSON-Objekt enthält nicht das erforderliche 'creatingLibrary' Element.");
        }

        if (!creatingLibraryProperty.TryGetProperty("name", out var nameProperty)
            || nameProperty.GetString() != "MediaInfoLib")
        {
            return Result.Failure<EssentialQuickTimeMetadata>("Das 'creatingLibrary' Element enthält nicht den erwarteten 'MediaInfoLib' Namen.");
        }

        if (!jsonElement.TryGetProperty("media", out var mediaProperty)
            || mediaProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<EssentialQuickTimeMetadata>("Das JSON-Objekt enthält nicht das erforderliche 'media' Element.");
        }

        if (!mediaProperty.TryGetProperty("track", out var trackProperty)
            || trackProperty.ValueKind != JsonValueKind.Array
            || trackProperty.GetArrayLength() == 0)
        {
            return Result.Failure<EssentialQuickTimeMetadata>("Das 'media' Element enthält kein Array 'track' oder das Array ist leer.");
        }

        var generalTrackElement = trackProperty[0];  // Get the first object from the track array

        return Result.Success(new EssentialQuickTimeMetadata(generalTrackElement));
    }

    public Result<YamlContent> GetYamlContent() => YamlContent.CreateFromMetadataSections(this);

    private static string? GetValue(JsonElement jsonObject, string propertyName)
    {
        if (jsonObject.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind != JsonValueKind.Null)
            {
                var value = property.GetString();
                return value;
            }
        }

        return null;
    }

    private static DateTime ParseDateTime(string? dateString)
    {
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
        {
            return result;
        }

        throw new FormatException($"Cannot parse '{dateString}' as a valid DateTime.");
    }

    private static TimeSpan? GetDuration(string? durationString)
    {
        if (string.IsNullOrWhiteSpace(durationString))
        {
            return null;
        }
        if (TimeSpan.TryParseExact(durationString, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, out var duration))
        {
            return duration;
        }

        return null;
    }

}
