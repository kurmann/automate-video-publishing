using System.Text.RegularExpressions;

namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Klasse zur Darstellung der Metadaten für ein Video.
/// </summary>
public class AppleVideoMetadata
{
    public static Result<AppleVideoMetadata> Create(AtomicParsleyMetadataReadResult atomicParsleyMetadata)
    {
        // Überprüfen Sie, ob atomicParsleyMetadata gültige Daten hat
        if (atomicParsleyMetadata == null || !atomicParsleyMetadata.Lines.Any())
        {
            return Result.Failure<AppleVideoMetadata>("AtomicParsleyMetadataReadResult ist ungültig oder hat keine Zeilen.");
        }

        var metadata = new AppleVideoMetadata();

        // Versuchen Sie, jede Zeile zu analysieren und in den entsprechenden Metadaten zu speichern
        foreach (var line in atomicParsleyMetadata.Lines)
        {
            if (TryParseAtomicParsleyResult(line.Value, out var atom, out var content))
            {
                switch (atom)
                {
                    case "©nam":
                        metadata.Title = content != null ? content : Maybe.None;
                        break;
                    case "©day":
                        if (DateTime.TryParse(content, out var date))
                        {
                            metadata.Day = date;
                        }
                        break;
                    case "©art":
                        metadata.Artist = content != null ? content : Maybe.None;
                        break;
                    case "©des":
                        metadata.Description = content != null ? content : Maybe.None;
                        break;
                    default:
                        break;
                }
            }
        }

        return Result.Success(metadata);
    }

    private static bool TryParseAtomicParsleyResult(string line, out string? atom, out string? content)
    {
        var match = Regex.Match(line, @"Atom ""(.*?)"" contains: (.*)");

        if (match.Success)
        {
            atom = match.Groups[1].Value;
            content = match.Groups[2].Value;
            return true;
        }

        atom = null;
        content = null;
        return false;
    }


    /// <summary>
    /// Setzt den Namen der TV Show auf dem "tvsh" Atom.
    /// </summary>
    public Maybe<string> TVShowName { get; set; }

    /// <summary>
    /// Setzt die TV-Seriennummer auf dem "tvsn" Atom.
    /// </summary>
    public Maybe<int> TVSeasonNum { get; set; }

    /// <summary>
    /// Setzt die TV-Episodennummer auf dem "tves" Atom.
    /// </summary>
    public Maybe<int> TVEpisodeNum { get; set; }

    /// <summary>
    /// Setzt den Namen der Person oder Firma, die die Datei auf dem "©enc" Atom kodiert hat.
    /// </summary>
    public Maybe<string> EncodedBy { get; set; }

    /// <summary>
    /// Setzt den Namen des Encoders auf dem "©too" Atom.
    /// </summary>
    public Maybe<string> EncodingTool { get; set; }

    /// <summary>
    /// Setzt die Beschreibung auf dem "desc" Atom.
    /// </summary>
    public Maybe<string> Description { get; set; }

    /// <summary>
    /// Setzt den Titel-Tag auf "moov.udta.meta.ilst.©nam.data".
    /// </summary>
    public Maybe<string> Title { get; set; }

    /// <summary>
    /// Setzt den Jahr-Tag auf "moov.udta.meta.ilst.©day.data".
    /// </summary>
    public Maybe<DateTime> Day { get; set; }

    /// <summary>
    /// Setzt den Künstler-Tag auf "moov.udta.meta.ilst.©ART.data".
    /// </summary>
    public Maybe<string> Artist { get; set; }

    /// <summary>
    /// Setzt den Copyright-Tag auf "moov.udta.meta.ilst.cprt.data".
    /// </summary>
    public Maybe<string> Copyright { get; set; }

    /// <summary>
    /// Setzt das iTunes "stik" Atom. Siehe "AtomicParsley --stik-list" für die vollständige Liste.
    /// </summary>
    public Maybe<AppleMediaType> Stik { get; set; }

    /// <summary>
    /// Setzt das "hdvd" Atom (true oder false um das Atom zu löschen).
    /// </summary>
    public Maybe<bool> HdVideo { get; set; }
}

/// <summary>
/// Enumeration für iTunes Medienformate.
/// </summary>
public enum AppleMediaType
{
    HomeVideo = 0,
    Normal = 1,
    Audiobook = 2,
    WhackedBookmark = 5,
    MusicVideo = 6,
    Movie = 9,  // Gilt auch für Kurzfilme
    TVShow = 10,
    Booklet = 11
}