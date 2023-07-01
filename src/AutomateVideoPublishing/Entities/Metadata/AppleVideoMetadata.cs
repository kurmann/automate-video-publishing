namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Klasse zur Darstellung der Metadaten für ein Video.
/// </summary>
public class VideoMetadata
{
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
    public Maybe<DateTime> Year { get; set; }

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