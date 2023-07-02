using System.Text.RegularExpressions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Repräsentiert die grundlegenden Daten zum Aktualisieren der Metadaten.
/// </summary>
public class UpdateMetadataBaseData : ValueObject
{
    // Der reguläre Ausdruck erkennt das ISO-Datum am Anfang des Dateinamens.
    private static readonly Regex DateRegex = new Regex(@"^(\d{4})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])");

    /// <summary>
    /// Die Beschreibung der Metadaten. Wenn der Wert None ist, war die Beschreibung leer oder nur Leerzeichen.
    /// </summary>
    public Maybe<string> Description { get; }

    /// <summary>
    /// Das Datum, das aus dem Dateinamen extrahiert wurde. Wenn der Wert None ist, konnte kein Datum aus dem Dateinamen extrahiert werden.
    /// </summary>
    public Maybe<DateTime> Date { get; }

    private UpdateMetadataBaseData(Maybe<string> description, Maybe<DateTime> date)
    {
        Description = description;
        Date = date;
    }

    /// <summary>
    /// Erstellt eine neue Instanz der UpdateMetadataBaseData-Klasse.
    /// </summary>
    /// <param name="description">Die Beschreibung der Metadaten.</param>
    /// <param name="filename">Der Dateiname, aus dem das Datum extrahiert werden soll.</param>
    /// <returns>Eine Instanz der UpdateMetadataBaseData-Klasse.</returns>
    public static Result<UpdateMetadataBaseData> Create(string description, string filename)
    {
        var maybeDescription = string.IsNullOrWhiteSpace(description) ? Maybe<string>.None : Maybe<string>.From(description);
        var maybeDate = ParseDateFromFilename(filename);

        return Result.Success(new UpdateMetadataBaseData(maybeDescription, maybeDate));
    }

    private static Maybe<DateTime> ParseDateFromFilename(string filename)
    {
        var match = DateRegex.Match(Path.GetFileNameWithoutExtension(filename));
        if (!match.Success) return Maybe<DateTime>.None;

        int year = int.Parse(match.Groups[1].Value);
        int month = int.Parse(match.Groups[2].Value);
        int day = int.Parse(match.Groups[3].Value);

        // 12 Stunden werden zum Datum hinzugefügt, um nicht bei Mitternacht zu landen.
        return Maybe<DateTime>.From(new DateTime(year, month, day, 12, 0, 0));
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Description.HasValue ? Description.Value : string.Empty;
        yield return Date.HasValue ? Date.Value : default(DateTime);
    }
}
