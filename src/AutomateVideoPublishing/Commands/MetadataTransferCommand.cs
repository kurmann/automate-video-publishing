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
                var pairResult = QuickTimeMpeg4Pair.Create(container.FileInfo.FullName, targetDirectoryPath);

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
                if (result != null)
                {
                    _broadcaster.OnNext(result);
                }
            });

        _quickTimeCommand.Execute(context);
        _broadcaster.OnCompleted();
    }

    private MetadataTransferResult TransferMetadata(QuickTimeMpeg4Pair pair)
    {
        var quickTimeMetadataContainer = pair.Source;
        var mpeg4 = pair.Target;

        var descriptionToBeTransferred = quickTimeMetadataContainer.Description;
        var descriptionTransferred = false;

        // Extract year from filename
        var filename = Path.GetFileNameWithoutExtension(quickTimeMetadataContainer.FileInfo.Name);
        var yearSubstring = filename.Substring(0, 4);
        var yearToBeTransferred = uint.TryParse(yearSubstring, out var year)
            ? year.ToString()
            : null;
        var yearTransferred = false;

        var mpeg4TagLibFile = TagLib.File.Create(mpeg4.FileInfo.FullName);

        // Check if metadata is different in target file
        if (descriptionToBeTransferred != null && mpeg4TagLibFile.Tag.Description != descriptionToBeTransferred)
        {
            mpeg4TagLibFile.Tag.Description = descriptionToBeTransferred;
            descriptionTransferred = true;
        }

        if (yearToBeTransferred != null && mpeg4TagLibFile.Tag.Year.ToString() != yearToBeTransferred)
        {
            mpeg4TagLibFile.Tag.Year = uint.Parse(yearToBeTransferred);
            yearTransferred = true;
        }

        mpeg4TagLibFile.Save();

        return new MetadataTransferResult
        {
            SourceFile = quickTimeMetadataContainer.FileInfo.FullName,
            TargetFile = mpeg4.FileInfo.FullName,
            DescriptionTransferred = descriptionToBeTransferred,
            YearTransferred = yearToBeTransferred,
            IsDescriptionTransferred = descriptionTransferred,
            IsYearTransferred = yearTransferred,
            IsFoundPair = true,
            DescriptionHasToBeTransferred = descriptionToBeTransferred != null,
            YearHasToBeTransferred = yearToBeTransferred != null,
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
    public string? YearTransferred { get; init; }
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

