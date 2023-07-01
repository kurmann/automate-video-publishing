namespace AutomateVideoPublishing.Entities.Metadata;

public interface IAtomicParsleyCommand
{
    public string ToString();
}

public class AtomicParsleyCommand
{
    private readonly string filePath;
    private readonly string atomicParsleyPath;

    public AtomicParsleyCommand(string filePath, string atomicParsleyPath = "AtomicParsley")
    {
        this.filePath = filePath;
        this.atomicParsleyPath = atomicParsleyPath;
    }

    public AtomicParsleyReadCommand ForReading() => new AtomicParsleyReadCommand(filePath, atomicParsleyPath);

    public AtomicParsleyWriteCommand ForWriting() => new AtomicParsleyWriteCommand(filePath, atomicParsleyPath);
}


public class AtomicParsleyReadCommand : IAtomicParsleyCommand
{
    private readonly List<string> arguments = new List<string>();
    private readonly string atomicParsleyPath;

    public AtomicParsleyReadCommand(string filePath, string atomicParsleyPath = "AtomicParsley")
    {
        this.atomicParsleyPath = atomicParsleyPath;
        arguments.Add(filePath);
    }

    public AtomicParsleyReadCommand WithMetadata()
    {
        arguments.Add("-t");
        return this;
    }

    public override string ToString() => $"{atomicParsleyPath} {string.Join(" ", arguments)}";

    public static implicit operator string(AtomicParsleyReadCommand cmd) => cmd.ToString();
}

public class AtomicParsleyWriteCommand : IAtomicParsleyCommand
{
    private readonly List<string> arguments = new List<string>();
    private readonly string atomicParsleyPath;

    public AtomicParsleyWriteCommand(string filePath, string atomicParsleyPath = "AtomicParsley")
    {
        this.atomicParsleyPath = atomicParsleyPath;
        arguments.Add(filePath);
    }

    public AtomicParsleyWriteCommand WithDescription(string description)
    {
        arguments.Add("--description");
        arguments.Add(description);
        return this;
    }

    public AtomicParsleyWriteCommand RemoveDay()
    {
        arguments.Add("--manualAtomRemove");
        arguments.Add("moov.udta.meta.ilst.©day");
        return this;
    }

    public AtomicParsleyWriteCommand WithDay(DateTime day)
    {
        arguments.Add("--year");
        arguments.Add(day.ToString("yyyy-MM-ddTHH:mm:ssZ"));
        return this;
    }

    public AtomicParsleyWriteCommand Overwrite()
    {
        arguments.Add("--overWrite");
        return this;
    }

    public override string ToString() => $"{atomicParsleyPath} {string.Join(" ", arguments)}";

    public static implicit operator string(AtomicParsleyWriteCommand cmd) => cmd.ToString();
}
