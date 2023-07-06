namespace AutomateVideoPublishing.Entities.AtomicParsley;

public class AtomicParsleyUpdateMetadataArguments
{
    public AtomicParsleyArguments Arguments { get; }

    private AtomicParsleyUpdateMetadataArguments(AtomicParsleyArguments arguments) => Arguments = arguments;

    public static AtomicParsleyUpdateMetadataArguments CreateOverwriteDescription(string filePath, string description)
    {
        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("description", description);

        return new AtomicParsleyUpdateMetadataArguments(arguments);
    }

    public static AtomicParsleyUpdateMetadataArguments CreateOverwriteDay(string filePath, DateTime day)
    {
        // Convert the DateTime to the required format (ISO 8601)
        string dayStr = day.ToString("yyyy-MM-ddTHH:mm:ssZ");

        var removeDayArgs = new AtomicParsleyArguments()
                                .AddFilePath(filePath)
                                .AddOption("--overWrite")
                                .AddTag("manualAtomRemove", "moov.udta.meta.ilst.©day");

        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("year", dayStr);

        return new AtomicParsleyUpdateMetadataArguments(arguments);
    }

    public override string ToString() => Arguments.ToString();

    public static implicit operator string?(AtomicParsleyUpdateMetadataArguments command) => command.Arguments.ToString();
}
