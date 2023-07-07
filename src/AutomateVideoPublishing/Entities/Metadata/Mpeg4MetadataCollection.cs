using System.Text.RegularExpressions;
using AutomateVideoPublishing.Entities.AtomicParsley;

namespace AutomateVideoPublishing.Entities.Metadata;

public class Mpeg4MetadataCollection
{
    private readonly Dictionary<string, string?> atoms;

    public IReadOnlyDictionary<string, string?> Metadata => atoms.AsReadOnly();

    public Maybe<string> Album => atoms.TryGetValue("©alb", out var album) && album != null ? Maybe<string>.From(album) : Maybe<string>.None;

    public Maybe<string> Description => atoms.TryGetValue("©des", out var description) && description != null ? Maybe<string>.From(description) : Maybe<string>.None;

    public Maybe<string> Name => atoms.TryGetValue("©nam", out var name) && name != null ? Maybe<string>.From(name) : Maybe<string>.None;

    private Mpeg4MetadataCollection(Dictionary<string, string?> atoms) => this.atoms = atoms;

    public static Result<Mpeg4MetadataCollection> Create(IEnumerable<AtomicParsleyMetadataReadOutputLine> lines)
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

    private static KeyValuePair<string, string?>? GetParsedAtomicParsleyLine(AtomicParsleyMetadataReadOutputLine line)
    {
        var match = Regex.Match(line.Value, @"Atom ""(.*?)"" contains: (.*)");

        if (match.Success)
        {
            var atom = match.Groups[1].Value;
            var value = match.Groups[2].Value;
            return new KeyValuePair<string, string?>(atom, value);
        }

        return null;
    }
}
