using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities.Metadata;

public class Mpeg4MetadataCollection
{
    private Dictionary<string, string?> _metadata = new();

    /// <summary>
    /// Liefert ein Dictionary, das die Atome und ihre entsprechenden Werte enthält.
    /// </summary>
    public IReadOnlyDictionary<string, string?> Metadata => _metadata;

    private Mpeg4MetadataCollection() { }

    public static Result<Mpeg4MetadataCollection> Create(IEnumerable<string> lines)
    {
        var metadataCollection = new Mpeg4MetadataCollection();

        foreach (var line in lines)
        {
            var (atom, value) = ParseLine(line);

            if (atom != null)
            {
                // Wenn das Atom bereits im Dictionary vorhanden ist, wird es nur hinzugefügt, wenn es einen Wert hat
                if (metadataCollection._metadata.ContainsKey(atom) && metadataCollection._metadata[atom] == null && value != null)
                {
                    metadataCollection._metadata[atom] = value;
                }
                // Wenn das Atom noch nicht im Dictionary vorhanden ist, wird es hinzugefügt
                else if (!metadataCollection._metadata.ContainsKey(atom))
                {
                    metadataCollection._metadata[atom] = value;
                }
            }
        }

        return Result.Success(metadataCollection);
    }

    private static (string? Atom, string? Value) ParseLine(string line)
    {
        var match = Regex.Match(line, @"Atom ""(.*?)"" contains: (.*)");

        if (match.Success)
        {
            return (match.Groups[1].Value, match.Groups[2].Value);
        }

        return (null, null);
    }
}
