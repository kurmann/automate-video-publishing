using System.Text.RegularExpressions;

namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Repräsentiert die Metadaten einer MP4-Datei und ihrer zugehörigen Apple-Metadaten.
/// </summary>
public class Mp4AndAppleMetadata
{
    /// <summary>
    /// Das '©nam'-Tag repräsentiert den Titel des Mediums.
    /// </summary>
    public Maybe<string> Title { get; set; }

    /// <summary>
    /// Das '©alb'-Tag repräsentiert den Albumnamen.
    /// </summary>
    public Maybe<string> Album { get; set; }

    /// <summary>
    /// Das '©gen'-Tag repräsentiert das Genre des Mediums.
    /// </summary>
    public Maybe<string> Genre { get; set; }

    /// <summary>
    /// Das '©day'-Tag repräsentiert das Veröffentlichungsdatum des Mediums.
    /// </summary>
    public Maybe<DateTime> ReleaseDate { get; set; }

    /// <summary>
    /// Das 'keyw'-Tag repräsentiert die mit dem Medium verknüpften Schlüsselwörter.
    /// </summary>
    public Maybe<string> Keywords { get; set; }

    /// <summary>
    /// Das '©des'-Tag repräsentiert die Beschreibung des Mediums.
    /// </summary>
    public Maybe<string> Description { get; set; }

    /// <summary>
    /// Das '©wrt'-Tag repräsentiert den Autor des Mediums.
    /// </summary>
    public Maybe<string> Writer { get; set; }

    /// <summary>
    /// Das '©prd'-Tag repräsentiert den Produzenten des Mediums.
    /// </summary>
    public Maybe<string> Producer { get; set; }

    /// <summary>
    /// Das '©dir'-Tag repräsentiert den Regisseur des Mediums.
    /// </summary>
    public Maybe<string> Director { get; set; }

    /// <summary>
    /// Das '©aut'-Tag repräsentiert den Autor des Mediums.
    /// </summary>
    public Maybe<string> Author { get; set; }

    /// <summary>
    /// Fügt Metadaten aus einer Zeile der Ausgabe von AtomicParsley hinzu.
    /// </summary>
    /// <param name="line">Eine Zeile der Ausgabe von AtomicParsley.</param>
    public Result AddFromAtomicParsley(string line)
    {
        var match = Regex.Match(line, @"Atom ""(.*)"" contains: (.*)");

        if (!match.Success)
        {
            return Result.Failure($"Following line is not a valid AtomicParsley output: {line}");
        }

        var tag = match.Groups[1].Value;
        var value = string.IsNullOrWhiteSpace(match.Groups[2].Value) ? Maybe.None : Maybe.From(match.Groups[2].Value);

        switch (tag)
        {
            case "©nam":
                Title = value;
                break;

            case "©alb":
                Album = value;
                break;

            case "©gen":
                Genre = value;
                break;

            case "©day":
                if (DateTime.TryParse(value.GetValueOrDefault(), out DateTime releaseDate))
                {
                    ReleaseDate = Maybe.From(releaseDate);
                }
                break;

            case "keyw":
                Keywords = value;
                break;

            case "©des":
                Description = value;
                break;

            case "©wrt":
                Writer = value;
                break;

            case "©prd":
                Producer = value;
                break;

            case "©dir":
                Director = value;
                break;

            case "©aut":
                Author = value;
                break;
        }

        return Result.Success($"Sucessfully parsed AtomicParsley output line: {line}");
    }
}
