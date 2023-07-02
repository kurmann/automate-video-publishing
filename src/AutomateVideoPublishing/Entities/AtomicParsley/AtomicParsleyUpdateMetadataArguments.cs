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

    public static Result<AtomicParsleyUpdateMetadataArguments> CreateOverwriteDay(string filePath, string day)
    {
        if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(day))
        {
            return Result.Failure<AtomicParsleyUpdateMetadataArguments>("FilePath and Day should not be null or empty.");
        }

        var removeDayArgs = new AtomicParsleyArguments()
                                .AddFilePath(filePath)
                                .AddOption("--overWrite")
                                .AddTag("manualAtomRemove", "moov.udta.meta.ilst.©day");
        
        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("year", day);

        return new AtomicParsleyUpdateMetadataArguments(arguments);
    }

    public static implicit operator string?(AtomicParsleyUpdateMetadataArguments command) => command.Arguments.ToString();
}
