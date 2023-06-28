using System.Text;

namespace AutomateVideoPublishing.Entities.Metadata;

public class MetadataTransferResult
{
    public FileInfo SourceFile { get; }
    public FileInfo TargetFile { get; }
    public MetadataAttribute MetadataAttributeName { get; }
    public TransferStatus Status { get; }

    public MetadataTransferResult(FileInfo sourceFile, FileInfo targetFile, MetadataAttribute metadataAttributeName, TransferStatus status)
    {
        SourceFile = sourceFile;
        TargetFile = targetFile;
        MetadataAttributeName = metadataAttributeName;
        Status = status;
    }

    public string SummaryMessage
    {
        get
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append($"Metadata transfer for file: {SourceFile.FullName} to file: {TargetFile.FullName}.");

            messageBuilder.Append(MetadataAttributeName.HasFlag(MetadataAttribute.Description)
                ? Status == TransferStatus.Success
                    ? " Description transferred successfully."
                    : " Description transfer failed."
                : " Description transfer not intended.");

            messageBuilder.Append(MetadataAttributeName.HasFlag(MetadataAttribute.Year)
                ? Status == TransferStatus.Success
                    ? " Year transferred successfully."
                    : " Year transfer failed."
                : " Year transfer not intended.");

            return messageBuilder.ToString();
        }
    }
}

[Flags]
public enum MetadataAttribute
{
    None = 0,
    Description = 1,
    Year = 2
}

public enum TransferStatus
{
    NotSpecified,
    NotRequired,
    Success,
    Failed
}
