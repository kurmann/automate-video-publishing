namespace AutomateVideoPublishing.Entities.Metadata;

public class AtomicParsleyMetadataReadResult
{
    private List<string> lines = new List<string>();

    public FileInfo FileInfo { get; }
    public IReadOnlyList<string> Lines => lines.AsReadOnly();
    private AtomicParsleyMetadataReadResult(FileInfo fileInfo) => FileInfo = fileInfo;

    public static Result<AtomicParsleyMetadataReadResult> Create(FileInfo fileInfo)
    {
        if (!fileInfo.Exists)
        {
            return Result.Failure<AtomicParsleyMetadataReadResult>($"File does not exist: {fileInfo.FullName}");
        }

        return Result.Success(new AtomicParsleyMetadataReadResult(fileInfo));
    }

    public void AddLine(string line) => lines.Add(line);
}
