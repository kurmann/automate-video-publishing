namespace AutomateVideoPublishing.Entities.AtomicParsley;

public class AtomicParsleyUpdateMetadataArguments
{
    public void OverwriteDescription(string filePath, string description)
    {
        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("description", description);
    }

    public void OverwriteDay(string filePath, string day)
    {
        RemoveDay(filePath);

        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("year", day);
    }

    private void RemoveDay(string filePath)
    {
        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("--overWrite")
                            .AddTag("manualAtomRemove", "moov.udta.meta.ilst.©day");
    }

    public static implicit operator string?(AtomicParsleyUpdateMetadataArguments command) => command.ToString();
}
