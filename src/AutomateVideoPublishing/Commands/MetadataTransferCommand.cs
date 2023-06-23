using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace AutomateVideoPublishing.Commands;

public class MetadataTransferCommand : ICommand<MetadataTransferResult>
{
    private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
    private readonly Subject<MetadataTransferResult> _broadcaster = new();

    public IObservable<MetadataTransferResult> WhenDataAvailable => _broadcaster.AsObservable();

    public MetadataTransferCommand(QuickTimeMetadataReadCommand quickTimeCommand) => _quickTimeCommand = quickTimeCommand;

    public void Execute(WorkflowContext context)
    {
        _quickTimeCommand.WhenDataAvailable
            .Select(container =>
            {
                var targetDirectoryPath = context.PublishedMpeg4Directory.Directory.FullName;
                var pairResult = QuickTimeMpeg4MetadataPair.Create(container.FileInfo.FullName, targetDirectoryPath);

                if (pairResult.IsFailure)
                {
                    _broadcaster.OnNext(new MetadataTransferResult
                    {
                        SourceFile = container.FileInfo.FullName,
                        IsFoundPair = false
                    });
                    return null;
                }

                return pairResult.Value;
            })
            .Where(pair => pair != null)
            .Subscribe(pair =>
            {
                var result = TransferMetadata(pair);
                _broadcaster.OnNext(result);
            });

        _quickTimeCommand.Execute(context);
        _broadcaster.OnCompleted();
    }

    private MetadataTransferResult TransferMetadata(QuickTimeMpeg4MetadataPair pair)
    {
        var quickTimeMetadataContainer = pair.QuickTimeMetadataContainer;
        var mpeg4 = pair.Mpeg4MetadataContainer;

        bool descriptionTransferred = false;
        bool yearTransferred = false;

        var mpeg4TagLibFile = TagLib.File.Create(mpeg4.FileInfo.FullName);

        // Check if metadata is different in target file
        if (!pair.IsDescriptionSame && quickTimeMetadataContainer.Description.HasValue)
        {
            mpeg4TagLibFile.Tag.Description = quickTimeMetadataContainer.Description.Value;
            descriptionTransferred = true;
        }

        if (!pair.IsYearSame && quickTimeMetadataContainer.YearByFilename.HasValue)
        {
            mpeg4TagLibFile.Tag.Year = quickTimeMetadataContainer.YearByFilename.Value;
            yearTransferred = true;
        }

        mpeg4TagLibFile.Save();

        return new MetadataTransferResult
        {
            SourceFile = quickTimeMetadataContainer.FileInfo.FullName,
            TargetFile = mpeg4.FileInfo.FullName,
            DescriptionTransferred = quickTimeMetadataContainer.Description.GetValueOrDefault(),
            YearTransferred = quickTimeMetadataContainer.YearByFilename.GetValueOrDefault(),
            IsDescriptionTransferred = descriptionTransferred,
            IsYearTransferred = yearTransferred,
            IsFoundPair = true,
            DescriptionHasToBeTransferred = !pair.IsDescriptionSame,
            YearHasToBeTransferred = !pair.IsYearSame,
        };
    }
}


public record MetadataTransferResult
{
    public string? SourceFile { get; init; }
    public string? TargetFile { get; init; }
    public TransferStatus DescriptionTransferStatus { get; init; }
    public TransferStatus YearTransferStatus { get; init; }

    public bool DescriptionHasToBeTransferred { get; set; }
    public bool YearHasToBeTransferred { get; set; }
    public string? DescriptionTransferred { get; init; }
    public uint? YearTransferred { get; init; }
    public bool IsDescriptionTransferred { get; set; }
    public bool IsYearTransferred { get; set; }
    public bool IsFoundPair { get; set; }
    public string? DescriptionError { get; init; }
    public string? YearError { get; init; }

    public string SummaryMessage
    {
        get
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append($"Metadata transfer for file: {SourceFile}.");

            messageBuilder.Append(IsFoundPair
                ? " Pair found."
                : " No Pair not found for QuickTime and MPEG-4 file with the same name to transfer metadata to.");

            if (DescriptionHasToBeTransferred)
            {
                messageBuilder.Append(IsDescriptionTransferred
                    ? $" Description transferred: {DescriptionTransferred}."
                    : " Description transfer failed or not needed.");
            }
            else
            {
                messageBuilder.Append(" Description transfer not intended.");
            }

            if (YearHasToBeTransferred)
            {
                messageBuilder.Append(IsYearTransferred
                    ? $" Year transferred: {YearTransferred}."
                    : " Year transfer failed or not needed.");
            }
            else
            {
                messageBuilder.Append(" Year transfer not intended.");
            }

            return messageBuilder.ToString();
        }
    }

    public enum TransferStatus
    {
        NotSpecified,
        NotRequired,
        Success,
        Failed
    }


}

