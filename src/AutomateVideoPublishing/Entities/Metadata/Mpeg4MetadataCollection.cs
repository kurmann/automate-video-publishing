using System.Text;
using System.Text.RegularExpressions;

namespace AutomateVideoPublishing.Entities.Metadata;

public class Mpeg4MetadataCollection
{
    private Dictionary<string, string?> _metadata = new();
    private readonly Dictionary<string, string?> atoms;

    /// <summary>
    /// Liefert ein Dictionary, das die Atome und ihre entsprechenden Werte enthält.
    /// </summary>
    public IReadOnlyDictionary<string, string?> Metadata => _metadata;

    private Mpeg4MetadataCollection(Dictionary<string, string?> atoms) => this.atoms = atoms;

    public static Result<Mpeg4MetadataCollection> Create(IEnumerable<string> lines)
    {
        var enumerator = lines.GetEnumerator();
        var atoms = new Dictionary<string, string?>();
        while (enumerator.MoveNext())
        {
            var line = enumerator.Current;
            var (atom, value) = ParseLine(line, enumerator);
            if (atom != null)
            {
                if (!atoms.ContainsKey(atom) || (atoms[atom] == null && value != null))
                {
                    atoms[atom] = value;
                }
            }

            // If the line doesn't start with "Atom", it means we've gone too far so we need to step back
            if (!line.StartsWith("Atom"))
            {
                // Step back
                while (enumerator.Current != null && !enumerator.Current.StartsWith("Atom"))
                {
                    enumerator.MoveNext();
                }
            }
        }

        var n = new KeyValuePair

        return Result.Success(new Mpeg4MetadataCollection(atoms));
    }




    public static (string? Atom, string? Value) ParseLine(string line, IEnumerator<string> enumerator)
    {
        var match = Regex.Match(line, @"Atom ""(.*?)"" contains: (.*)");
        string? atom = null;
        var valueBuilder = new StringBuilder();

        if (match.Success)
        {
            atom = match.Groups[1].Value;
            valueBuilder.Append(match.Groups[2].Value);
        }

        // Check for multi-line content
        while (enumerator.MoveNext())
        {
            line = enumerator.Current;

            // If the line starts with "Atom", it's a new atom so stop collecting lines
            if (line.StartsWith("Atom"))
            {
                break;
            }

            // Append the current line to the value
            valueBuilder.AppendLine(line);
        }

        return (atom, valueBuilder.ToString());
    }





}
