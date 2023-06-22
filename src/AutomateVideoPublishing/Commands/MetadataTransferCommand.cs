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

    private MetadataTransferResult? TransferMetadata(QuickTimeMpeg4Pair? pair)
    {
        if (pair == null)
        {
            throw new ArgumentNullException(nameof(pair));
        }

        var quickTimeMetadataContainer = pair.Source;
        var mpeg4 = pair.Target;
        var descriptionTransferred = false;
        var yearTransferred = false;
        var isFoundPair = false;

        try
        {
            var mpeg4TagLibFile = TagLib.File.Create(pair.Target.FileInfo.FullName);

            var descriptionOrDefault = quickTimeMetadataContainer.Description;
            if (!string.IsNullOrWhiteSpace(descriptionOrDefault))
            {
                mpeg4TagLibFile.Tag.Description = descriptionOrDefault;
                descriptionTransferred = true;
            }

            // Attempt to extract year from filename
            var filename = Path.GetFileNameWithoutExtension(quickTimeMetadataContainer.FileInfo.Name);
            var yearSubstring = filename.Substring(0, 4);
            if (uint.TryParse(yearSubstring, out var year))
            {
                mpeg4TagLibFile.Tag.Year = year;
                yearTransferred = true;
            }

            isFoundPair = true;
            mpeg4TagLibFile.Save();

            return new MetadataTransferResult
            {
                SourceFile = quickTimeMetadataContainer.FileInfo.FullName,
                TargetFile = mpeg4.FileInfo.FullName,
                DescriptionTransferred = descriptionOrDefault,
                YearTransferred = year.ToString(),
                IsDescriptionTransferred = descriptionTransferred,
                IsYearTransferred = yearTransferred,
                IsFoundPair = isFoundPair
            };
        }
        catch (Exception ex)
        {
            _broadcaster.OnError(ex);
            return null;
        }
    }

}

public record MetadataTransferResult
{
    public string? SourceFile { get; init; }
    public string? TargetFile { get; init; }
    public string? DescriptionTransferred { get; init; }
    public string? YearTransferred { get; init; }
    public bool IsDescriptionTransferred { get; init; }
    public bool IsYearTransferred { get; init; }
    public bool IsFoundPair { get; init; }

    public string SummaryMessage
    {
        get
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.Append($"Metadata transfer for file: {SourceFile}.");

            messageBuilder.Append(IsFoundPair 
                ? " Pair found." 
                : " No Pair not found for QuickTime and MPEG-4 file with the same name to transfer metadata to.");

            messageBuilder.Append(IsDescriptionTransferred 
                ? $" Description: {DescriptionTransferred}." 
                : " Description not transferred.");

            messageBuilder.Append(IsYearTransferred 
                ? $" Year: {YearTransferred}." 
                : " Year not transferred.");

            return messageBuilder.ToString();
        }
    }


}

