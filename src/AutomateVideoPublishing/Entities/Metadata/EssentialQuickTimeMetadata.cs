using System.Globalization;
using System.Text.Json;

namespace AutomateVideoPublishing.Entities.Metadata;

public class EssentialQuickTimeMetadata
{
    public string? Title { get; }
    public string? Description { get; }
    public string? Album { get; }
    public string? WidthString { get; }
    public string? HeightString { get; }
    public string? Genre { get; set; }
    public string? FrameRateString { get; }
    public string? DurationString { get; }
    public TimeSpan? Duration => GetDuration(DurationString);
    public string? Format { get; }
    public string? FormatProfile { get; }
    public string? Producer { get; }
    public string? InternetMediaType { get; }
    public string? BitRateVideoString { get; }
    public string? ChromaSubsampling { get; }
    public string? FileSize { get; }
    public string? EncodedDateString { get; }

    private EssentialQuickTimeMetadata(JsonElement jsonObject)
    {
        Title = TryGetValue(jsonObject, "Title");
        Description = TryGetValue(jsonObject, "Description");
        Album = TryGetExtraValue(jsonObject, "com_apple_quicktime_album");
        WidthString = TryGetValue(jsonObject, "Width", MediaInfoTrackType.Video);
        HeightString = TryGetValue(jsonObject, "Height", MediaInfoTrackType.Video);
        Genre = TryGetExtraValue(jsonObject, "com_apple_quicktime_genre");
        FrameRateString = TryGetValue(jsonObject, "FrameRate", MediaInfoTrackType.Video);
        DurationString = TryGetValue(jsonObject, "Duration_String3");
        Format = TryGetValue(jsonObject, "Format", MediaInfoTrackType.Video);
        FormatProfile = TryGetValue(jsonObject, "Format_Profile", MediaInfoTrackType.Video);
        Producer = TryGetExtraValue(jsonObject, "com_apple_quicktime_producer");
        InternetMediaType = TryGetValue(jsonObject, "InternetMediaType");
        BitRateVideoString = TryGetValue(jsonObject, "BitRate", MediaInfoTrackType.Video);
        ChromaSubsampling = TryGetValue(jsonObject, "ChromaSubsampling", MediaInfoTrackType.Video);
        FileSize = TryGetValue(jsonObject, "FileSize");
        EncodedDateString = TryGetValue(jsonObject, "Encoded_Date");
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

        return Result.Success(new EssentialQuickTimeMetadata(trackProperty));
    }

    public Result<YamlContent> GetYamlContent() => YamlContent.CreateFromMetadataSections(this);

    /// <summary>
    /// Liest den Wert eines bestimmten Attributs aus dem gegebenen Track aus.
    /// </summary>
    /// <param name="trackArray"></param>
    /// <param name="propertyName"></param>
    /// <param name="trackType"></param>
    /// <returns></returns>
    public static string? TryGetValue(JsonElement trackArray, string propertyName, MediaInfoTrackType trackType = MediaInfoTrackType.General)
    {
        var trackTypeName = trackType.ToString().ToLowerInvariant();  // Convert Enum value to lower-case string

        foreach (var track in trackArray.EnumerateArray())
        {
            if (track.TryGetProperty("@type", out var typeProperty) && typeProperty.GetString()?.ToLowerInvariant() == trackTypeName)
            {
                if (track.TryGetProperty(propertyName, out var property))
                {
                    if (property.ValueKind != JsonValueKind.Null)
                    {
                        var value = property.GetString();
                        return value;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Liest das "extra"-Attribut eines bestimmten Tracks aus. Dieses wird von Apple benutzt für spezifische Metadaten wie "Album".
    /// </summary>
    /// <param name="trackArray"></param>
    /// <param name="extraPropertyName"></param>
    /// <param name="trackType"></param>
    /// <returns></returns>
    public static string? TryGetExtraValue(JsonElement trackArray, string extraPropertyName, MediaInfoTrackType trackType = MediaInfoTrackType.General)
    {
        // Ensure the passed JsonElement is actually an array
        if (trackArray.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        // Iterate through each track in the array
        foreach (var trackElement in trackArray.EnumerateArray())
        {
            // Try to get the '@type' property of the current track
            if (trackElement.TryGetProperty("@type", out var typeElement)
                && typeElement.ValueKind == JsonValueKind.String
                && typeElement.GetString()?.Equals(trackType.ToString(), StringComparison.OrdinalIgnoreCase) == true)
            {
                // Found the track with the matching type, now look for the 'extra' property
                if (trackElement.TryGetProperty("extra", out var extraElement)
                    && extraElement.ValueKind == JsonValueKind.Object)
                {
                    // Try to get the specified 'extra' property
                    if (extraElement.TryGetProperty(extraPropertyName, out var extraProperty))
                    {
                        // Return the value of the 'extra' property
                        if (extraProperty.ValueKind != JsonValueKind.Null)
                        {
                            return extraProperty.GetString();
                        }
                    }
                }
            }
        }

        // No matching track and/or 'extra' property found
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

public enum MediaInfoTrackType
{
    General,
    Video,
    Audio,
    Other,
    Menu
}
