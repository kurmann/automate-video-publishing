using System.Text;

namespace AutomateVideoPublishing.Entities;

public class MetadataTransferResult
{
    public string SourceFile { get; }
    public string TargetFile { get; }
    public TransferStatus DescriptionTransferStatus { get; }
    public TransferStatus YearTransferStatus { get; }
    public Maybe<string> DescriptionTransferred { get; }
    public Maybe<uint> YearTransferred { get; }
    public bool IsDescriptionTransferred { get; }
    public bool IsYearTransferred { get; }
    public bool IsFoundPair { get; }

    private MetadataTransferResult(
        string sourceFile,
        string targetFile,
        TransferStatus descriptionTransferStatus,
        TransferStatus yearTransferStatus,
        Maybe<string> descriptionTransferred,
        Maybe<uint> yearTransferred,
        bool isDescriptionTransferred,
        bool isYearTransferred,
        bool isFoundPair)
    {
        SourceFile = sourceFile;
        TargetFile = targetFile;
        DescriptionTransferStatus = descriptionTransferStatus;
        YearTransferStatus = yearTransferStatus;
        DescriptionTransferred = descriptionTransferred;
        YearTransferred = yearTransferred;
        IsDescriptionTransferred = isDescriptionTransferred;
        IsYearTransferred = isYearTransferred;
        IsFoundPair = isFoundPair;
    }

    public static MetadataTransferResult Create(
        string sourceFile,
        string targetFile,
        TransferStatus descriptionTransferStatus,
        TransferStatus yearTransferStatus,
        Maybe<string> descriptionTransferred,
        Maybe<uint> yearTransferred,
        bool isDescriptionTransferred,
        bool isYearTransferred,
        bool isFoundPair)
    {
        return new MetadataTransferResult(
            sourceFile,
            targetFile,
            descriptionTransferStatus,
            yearTransferStatus,
            descriptionTransferred,
            yearTransferred,
            isDescriptionTransferred,
            isYearTransferred,
            isFoundPair);
    }

    public enum TransferStatus
    {
        NotSpecified,
        NotRequired,
        Success,
        Failed
    }

    public string SummaryMessage
    {
        get
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append($"Metadata transfer for file: {SourceFile}.");

            messageBuilder.Append(IsFoundPair
                ? " Pair found."
                : " No pair found for QuickTime and MPEG-4 file with the same name to transfer metadata to.");

            if (DescriptionTransferStatus != TransferStatus.NotRequired)
            {
                if (DescriptionTransferred.HasValue)
                {
                    messageBuilder.Append(DescriptionTransferStatus == TransferStatus.Success
                        ? $" Description transferred: {DescriptionTransferred.Value}."
                        : " Description transfer failed.");
                }
                else
                {
                    messageBuilder.Append(" Description not found in source.");
                }
            }
            else
            {
                messageBuilder.Append(" Description transfer not intended.");
            }

            if (YearTransferStatus != TransferStatus.NotRequired)
            {
                if (YearTransferred.HasValue)
                {
                    messageBuilder.Append(YearTransferStatus == TransferStatus.Success
                        ? $" Year transferred: {YearTransferred.Value}."
                        : " Year transfer failed.");
                }
                else
                {
                    messageBuilder.Append(" Year not found in source.");
                }
            }
            else
            {
                messageBuilder.Append(" Year transfer not intended.");
            }

            return messageBuilder.ToString();
        }
    }

}
