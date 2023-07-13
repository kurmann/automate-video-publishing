namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Klasse zur Repräsentation und Verarbeitung von Quicktime-Metadaten.
/// Extrahiert wichtige Metadaten aus einer `MediaInfoMetadataLineOutput`-Instanz und ermöglicht den einfachen Zugriff auf diese.
/// </summary>
public class ParsedQuicktimeMetadata
{
    // Properties
    public Maybe<string> Title { get; }
    public Maybe<string> Description { get; }
    public @object RawMetadata { get; }

    // Private Constructor
    private ParsedQuicktimeMetadata(@object sections)
    {
        RawMetadata = sections;
        Title = ExtractValueFromDictionary(sections.General, "Title");
        Description = ExtractValueFromDictionary(sections.General, "Description");
    }

    /// <summary>
    /// Versucht, einen bestimmten Wert, identifiziert durch 'keyName', aus einer spezifischen Sektion, 
    /// identifiziert durch 'sectionName', zu extrahieren.
    /// Gibt die Werte aller übereinstimmenden Schlüssel als einen einzigen, durch Semikolons 
    /// getrennten String zurück oder Maybe<string>.None, wenn kein übereinstimmender Schlüssel gefunden wurde.
    /// </summary>
    /// <param name="sectionName">Der Name der Section (bspw. "Video" oder "Audio"</param>
    /// <param name="keyName">Der Schlüsselname bspw. "Title" oder "Duration"</param>
    /// <param name="sections">Die Sektionen, aus denen der Wert extrahiert werden soll</param>
    /// <returns></returns>
    private static Maybe<string> ExtractSingleValueFromSection(string sectionName, string keyName, IReadOnlyDictionary<string, List<KeyValuePair<string, string?>>> sections)
    {
        if (sections.TryGetValue(sectionName, out var section))
        {
            var keyValuePairs = section.Where(pair => pair.Key == keyName).ToList();
            if (keyValuePairs.Any())
            {
                return string.Join("; ", keyValuePairs.Select(pair => pair.Value));
            }
        }
        return Maybe<string>.None;
    }

    /// <summary>
    /// Erstellt eine Instanz von `ParsedQuicktimeMetadata` aus einer Instanz von `MediaInfoMetadataLineOutput`.
    /// Extrahiert die Werte für 'Title' und 'Description' aus der 'General'-Sektion der übergebenen Metadaten.
    /// </summary>
    /// <param name="mediaInfoMetadataLineOutput">Die Metadaten, aus denen die `ParsedQuicktimeMetadata`-Instanz erstellt werden soll.</param>
    /// <returns></returns>
    public static Result<ParsedQuicktimeMetadata> Create(MediaInfoMetadataLineOutput mediaInfoMetadataLineOutput)
    {
        if (mediaInfoMetadataLineOutput == null)
            return Result.Failure<ParsedQuicktimeMetadata>("mediaInfoMetadataLineOutput cannot be null.");

        // Erstelle Sections
        var sections = new @object
        {
            General = mediaInfoMetadataLineOutput.Sections.ContainsKey("General")
                ? MergeKeyValuePairListIntoDictionary(mediaInfoMetadataLineOutput.Sections["General"])
                : null,
            Video = mediaInfoMetadataLineOutput.Sections.ContainsKey("Video")
                ? MergeKeyValuePairListIntoDictionary(mediaInfoMetadataLineOutput.Sections["Video"])
                : null,
            Audio = mediaInfoMetadataLineOutput.Sections.ContainsKey("Audio")
                ? MergeKeyValuePairListIntoDictionary(mediaInfoMetadataLineOutput.Sections["Audio"])
                : null,
            Other = mediaInfoMetadataLineOutput.Sections.ContainsKey("Other")
                ? MergeKeyValuePairListIntoDictionary(mediaInfoMetadataLineOutput.Sections["Other"])
                : null,
            Menu = mediaInfoMetadataLineOutput.Sections.ContainsKey("Menu")
                ? MergeKeyValuePairListIntoDictionary(mediaInfoMetadataLineOutput.Sections["Menu"])
                : null,
        };

        return new ParsedQuicktimeMetadata(sections);
    }

    private static Dictionary<string, string?> MergeKeyValuePairListIntoDictionary(List<KeyValuePair<string, string?>> list)
    {
        var dict = new Dictionary<string, string?>();

        foreach (var pair in list)
        {
            if (!dict.ContainsKey(pair.Key))
            {
                dict[pair.Key] = pair.Value;
            }
            else if (pair.Value != null)
            {
                dict[pair.Key] += "; " + pair.Value;
            }
        }

        return dict;
    }

    private static Maybe<string> ExtractValueFromDictionary(Dictionary<string, string?>? dictionary, string key)
    {
        if (dictionary == null)
        {
            return Maybe<string>.None;
        }
        dictionary.TryGetValue(key, out string? value);
        return string.IsNullOrWhiteSpace(value) ? Maybe.None : Maybe.From(value);
    }
}

public record @object
{
    public Dictionary<string, string?>? General { get; set; }
    public Dictionary<string, string?>? Video { get; set; }
    public Dictionary<string, string?>? Audio { get; set; }
    public Dictionary<string, string?>? Other { get; set; }
    public Dictionary<string, string?>? Menu { get; set; }
}
