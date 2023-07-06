using AutomateVideoPublishing.Entities.AtomicParsley;

namespace AutomateVideoPublishing.Entities.Metadata;

public class AtomicParsleyMetadataReadResult
{
    private List<AtomicParsleyMetadataReadOutputLine> lines = new ();

    public FileInfo FileInfo { get; }
    public IReadOnlyList<AtomicParsleyMetadataReadOutputLine> Lines => lines.AsReadOnly();
    private AtomicParsleyMetadataReadResult(FileInfo fileInfo) => FileInfo = fileInfo;

    public static AtomicParsleyMetadataReadResult Create(FileInfo fileInfo) => new AtomicParsleyMetadataReadResult(fileInfo);

    public void AddLine(AtomicParsleyMetadataReadOutputLine line) => lines.Add(line);
}
