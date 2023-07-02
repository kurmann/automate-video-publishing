using System.Text;
using System.Text.RegularExpressions;

namespace AutomateVideoPublishing.Entities.Metadata;

public class Mpeg4MetadataCollection
{
    private readonly Dictionary<string, string?> atoms;

    public IReadOnlyDictionary<string, string?> Metadata => atoms.AsReadOnly();

    private Mpeg4MetadataCollection(Dictionary<string, string?> atoms) => this.atoms = atoms;

    public static Result<Mpeg4MetadataCollection> Create(IEnumerable<string> lines)
    {
        var atoms = new Dictionary<string, string?>();
        string lastAtom = string.Empty;

        foreach (var line in lines)
        {
            var parsedLine = GetParsedAtomicParsleyLine(line);

            if (parsedLine.HasValue)
            {
                lastAtom = parsedLine.Value.Key;
                atoms[lastAtom] = parsedLine.Value.Value;
            }
            else
            {
                if (atoms.ContainsKey(lastAtom))
                {
                    atoms[lastAtom] += Environment.NewLine + line;
                }
            }
        }

        return Result.Success(new Mpeg4MetadataCollection(atoms));
    }

    private static KeyValuePair<string, string?>? GetParsedAtomicParsleyLine(string line)
    {
        var match = Regex.Match(line, @"Atom ""(.*?)"" contains: (.*)");

        if (match.Success)
        {
            var atom = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            return new KeyValuePair<string, string?>(atom, value);
        }

        return null;
    }
}
