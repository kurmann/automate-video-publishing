namespace AutomateVideoPublishing.Entities.Metadata;

public class AtomicParsleyCommand
{
    private const string AtomicParsleyPath = "/usr/local/bin/AtomicParsley";
    private readonly List<string> arguments = new List<string>();

    public AtomicParsleyCommand(string filePath) => arguments.Add(filePath);

    public AtomicParsleyCommand WithMetadata()
    {
        arguments.Add("-t");
        return this;
    }

    public AtomicParsleyCommand WithDescription(string description)
    {
        arguments.Add("--description");
        arguments.Add(description);
        return this;
    }

    public AtomicParsleyCommand RemoveDay()
    {
        arguments.Add("--manualAtomRemove");
        arguments.Add("moov.udta.meta.ilst.©day");
        return this;
    }

    public AtomicParsleyCommand WithDay(string day)
    {
        arguments.Add("--year");
        arguments.Add(day);
        return this;
    }

    public AtomicParsleyCommand Overwrite()
    {
        arguments.Add("--overWrite");
        return this;
    }

    public override string ToString() => $"{AtomicParsleyPath} {string.Join(" ", arguments)}";

    public static implicit operator string(AtomicParsleyCommand cmd) => cmd.ToString();
}
