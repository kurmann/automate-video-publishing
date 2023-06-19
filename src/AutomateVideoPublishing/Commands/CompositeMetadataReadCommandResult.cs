namespace AutomateVideoPublishing.Commands;

public class CompositeMetadataReadCommandResult
{
    public string? GeneralMessage { get; set; }
    public List<Result<QuickTimeMetadataContainer>> QuickTimeMetadataContainers { get; set; } = new List<Result<QuickTimeMetadataContainer>>();
    public List<Result<Mpeg4MetadataContainer>> Mpeg4MetadataContainers { get; set; } = new List<Result<Mpeg4MetadataContainer>>();
    public List<Result<FileInfo>> CreatedJsonFiles { get; set;} = new List<Result<FileInfo>>();
}