using System.Reactive.Linq;
using System.Reactive.Subjects;

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
                    _broadcaster.OnNext(MetadataTransferResult.CreatePairNotFound(container.FileInfo.FullName));
                    return null;
                }

                return pairResult.Value;
            })
            .Where(pair => pair != null)
            .Subscribe(pair =>
            {
                var result = TransferMetadata(pair!);
                _broadcaster.OnNext(result);
            });

        _quickTimeCommand.Execute(context);
        _broadcaster.OnCompleted();
    }

    private MetadataTransferResult TransferMetadata(QuickTimeMpeg4MetadataPair pair)
    {
        var quickTimeMetadataContainer = pair.QuickTimeMetadataContainer;
        var mpeg4 = pair.Mpeg4MetadataContainer;

        var mpeg4TagLibFile = TagLib.File.Create(mpeg4.FileInfo.FullName);

        // Check if metadata is different in target file
        var descriptionTransferred = !pair.IsDescriptionSame && quickTimeMetadataContainer.Description.HasValue
            ? quickTimeMetadataContainer.Description.Value
            : null;

        var yearTransferred = !pair.IsYearSame && quickTimeMetadataContainer.YearByFilename.HasValue
            ? quickTimeMetadataContainer.YearByFilename.Value
            : (uint?)null;

        bool isDescriptionTransferred = descriptionTransferred != null;
        bool isYearTransferred = yearTransferred.HasValue;

        if (descriptionTransferred != null)
        {
            mpeg4TagLibFile.Tag.Description = descriptionTransferred;
        }

        if (yearTransferred.HasValue)
        {
            mpeg4TagLibFile.Tag.Year = yearTransferred.Value;
        }

        mpeg4TagLibFile.Save();

        return MetadataTransferResult.Create(
            sourceFile: quickTimeMetadataContainer.FileInfo.FullName,
            targetFile: mpeg4.FileInfo.FullName,
            descriptionTransferStatus: isDescriptionTransferred
                ? MetadataTransferResult.TransferStatus.Success
                : MetadataTransferResult.TransferStatus.NotRequired,
            yearTransferStatus: isYearTransferred
                ? MetadataTransferResult.TransferStatus.Success
                : MetadataTransferResult.TransferStatus.NotRequired,
            descriptionTransferred: descriptionTransferred != null ? Maybe.From(descriptionTransferred) : Maybe<string>.None,
            yearTransferred: yearTransferred.HasValue && yearTransferred.HasValue
                ? Maybe.From(yearTransferred.Value)
                : Maybe<uint>.None,
            isDescriptionTransferred: isDescriptionTransferred,
            isYearTransferred: isYearTransferred,
            isFoundPair: true
        );
    }




}
