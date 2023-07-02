namespace AutomateVideoPublishing.Entities.AtomicParsley;

public class AtomicParsleyUpdateMetadataArguments
{
    public AtomicParsleyArguments Arguments { get; }

    private AtomicParsleyUpdateMetadataArguments(AtomicParsleyArguments arguments) => Arguments = arguments;

    public static Result<AtomicParsleyUpdateMetadataArguments> CreateOverwriteDescription(string filePath, string description)
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(description))
        {
            return Result.Failure<AtomicParsleyUpdateMetadataArguments>("FilePath and Description should not be null or empty.");
        }

        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("description", description);

        return new AtomicParsleyUpdateMetadataArguments(arguments);
    }

    public static Result<AtomicParsleyUpdateMetadataArguments> CreateOverwriteDay(string filePath, DateTime day)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return Result.Failure<AtomicParsleyUpdateMetadataArguments>("FilePath should not be null or empty.");
        }

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


    public static implicit operator string?(AtomicParsleyUpdateMetadataArguments command) => command.Arguments.ToString();
}
