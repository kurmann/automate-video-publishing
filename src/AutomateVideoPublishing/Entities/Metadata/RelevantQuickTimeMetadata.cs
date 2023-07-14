using System.Globalization;
using System.Text.Json;

namespace AutomateVideoPublishing.Entities.Metadata;

public class RelevantQuickTimeMetadata
{
    public RelevantQuickTimeAttributes Attributes { get; set; }
    public JsonDocument RawMetadata { get; set; }

    private RelevantQuickTimeMetadata(RelevantQuickTimeAttributes attributes, JsonDocument jsonObject)
    {
        Attributes = attributes;
        RawMetadata = jsonObject;
    }

    public static Result<RelevantQuickTimeMetadata> Create(JsonDocument? jsonDocument)
    {
        if (jsonDocument == null)
        {
            return Result.Failure<RelevantQuickTimeMetadata>("JSON Dokument darf nicht leer sein");
        }

        var jsonElement = jsonDocument.RootElement;

        if (!jsonElement.TryGetProperty("creatingLibrary", out var creatingLibraryProperty)
            || creatingLibraryProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<RelevantQuickTimeMetadata>("Das JSON-Objekt enthält nicht das erforderliche 'creatingLibrary' Element.");
        }

        if (!creatingLibraryProperty.TryGetProperty("name", out var nameProperty)
            || nameProperty.GetString() != "MediaInfoLib")
        {
            return Result.Failure<RelevantQuickTimeMetadata>("Das 'creatingLibrary' Element enthält nicht den erwarteten 'MediaInfoLib' Namen.");
        }

        if (!jsonElement.TryGetProperty("media", out var mediaProperty)
            || mediaProperty.ValueKind != JsonValueKind.Object)
        {
            return Result.Failure<RelevantQuickTimeMetadata>("Das JSON-Objekt enthält nicht das erforderliche 'media' Element.");
        }

        if (!mediaProperty.TryGetProperty("track", out var trackProperty)
            || trackProperty.ValueKind != JsonValueKind.Array
            || trackProperty.GetArrayLength() == 0)
        {
            return Result.Failure<RelevantQuickTimeMetadata>("Das 'media' Element enthält kein Array 'track' oder das Array ist leer.");
        }

        var attributes = new RelevantQuickTimeAttributes
        {
            Title = TryGetValue(trackProperty, "Title"),
            Description = TryGetValue(trackProperty, "Description"),
            Album = TryGetExtraValue(trackProperty, "com_apple_quicktime_album"),
            Width = TryGetValue(trackProperty, "Width", MediaInfoTrackType.Video),
            Height = TryGetValue(trackProperty, "Height", MediaInfoTrackType.Video),
            Genre = TryGetExtraValue(trackProperty, "com_apple_quicktime_genre"),
            FrameRate = TryGetValue(trackProperty, "FrameRate", MediaInfoTrackType.Video),
            Duration = TryGetValue(trackProperty, "Duration_String3"),
            Format = TryGetValue(trackProperty, "Format", MediaInfoTrackType.Video),
            FormatProfile = TryGetValue(trackProperty, "Format_Profile", MediaInfoTrackType.Video),
            Producer = TryGetExtraValue(trackProperty, "com_apple_quicktime_producer"),
            InternetMediaType = TryGetValue(trackProperty, "InternetMediaType"),
            VideoBitRate = TryGetValue(trackProperty, "BitRate", MediaInfoTrackType.Video),
            ChromaSubsampling = TryGetValue(trackProperty, "ChromaSubsampling", MediaInfoTrackType.Video),
            FileSize = TryGetValue(trackProperty, "FileSize"),
            EncodedDate = TryGetValue(trackProperty, "Encoded_Date"),
            CommaSeparatedKeywords = TryGetExtraValue(trackProperty, "com_apple_quicktime_keywords")
        };

        return Result.Success(new RelevantQuickTimeMetadata(attributes, jsonDocument));
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

public record RelevantQuickTimeAttributes
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Album { get; init; }
    public string? Width { get; init; }
    public string? Height { get; init; }
    public string? Genre { get; init; }
    public string? FrameRate { get; init; }
    public string? Duration { get; init; }
    public string? Format { get; init; }
    public string? FormatProfile { get; init; }
    public string? Producer { get; init; }
    public string? InternetMediaType { get; init; }
    public string? VideoBitRate { get; init; }
    public string? ChromaSubsampling { get; init; }
    public string? FileSize { get; init; }
    public string? EncodedDate { get; init; }
    public string? CommaSeparatedKeywords { get; init; }
}