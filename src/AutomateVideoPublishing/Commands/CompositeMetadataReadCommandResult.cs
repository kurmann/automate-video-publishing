namespace AutomateVideoPublishing.Commands;

public class CompositeMetadataReadCommandResult
{
    public string? GeneralMessage { get; set; }
    public List<QuickTimeMetadataContainer> QuickTimeMetadataContainers { get; set; } = new List<QuickTimeMetadataContainer>();
    public List<Mpeg4MetadataContainer> Mpeg4MetadataContainers { get; set; } = new List<Mpeg4MetadataContainer>();
    public Dictionary<FileInfo, string> FailedFiles { get; set; } = new Dictionary<FileInfo, string>();

}