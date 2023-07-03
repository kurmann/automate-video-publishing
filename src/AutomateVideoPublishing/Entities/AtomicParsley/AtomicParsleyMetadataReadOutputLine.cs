namespace AutomateVideoPublishing.Entities.AtomicParsley;

public class AtomicParsleyMetadataReadOutputLine : ValueObject
{
    public string Value { get; }

    private AtomicParsleyMetadataReadOutputLine(string value) => Value = value;

    public static AtomicParsleyMetadataReadOutputLine Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            new AtomicParsleyMetadataReadOutputLine(string.Empty);
        }

        return new AtomicParsleyMetadataReadOutputLine(value);
    }

    protected override IEnumerable<string> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(AtomicParsleyMetadataReadOutputLine outputLine) => outputLine.Value;
}
