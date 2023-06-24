namespace AutomateVideoPublishing.Entities.Metadata;

public class MetadataTransferAnalyzer
{
    private QuickTimeMpeg4MetadataPair _beforeTransfer;
    private QuickTimeMpeg4MetadataPair _afterTransfer;

    public MetadataTransferAnalyzer(QuickTimeMpeg4MetadataPair beforeTransfer, QuickTimeMpeg4MetadataPair afterTransfer)
    {
        _beforeTransfer = beforeTransfer;
        _afterTransfer = afterTransfer;
    }

    public MetadataTransferResult Analyze()
    {
        var quickTimeMetadataContainer = _beforeTransfer.QuickTimeMetadataContainer;
        var mpeg4 = _afterTransfer.Mpeg4MetadataContainer;

        var descriptionTransferred = !_beforeTransfer.IsDescriptionSame && quickTimeMetadataContainer.Description.HasValue
            ? quickTimeMetadataContainer.Description.Value
            : null;

        var yearTransferred = !_beforeTransfer.IsYearSame && quickTimeMetadataContainer.YearByFilename.HasValue
            ? quickTimeMetadataContainer.YearByFilename.Value
            : (uint?)null;

        bool isDescriptionTransferred = descriptionTransferred != null;
        bool isYearTransferred = yearTransferred.HasValue;

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
