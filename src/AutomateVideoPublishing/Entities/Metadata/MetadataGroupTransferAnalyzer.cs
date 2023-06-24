namespace AutomateVideoPublishing.Entities.Metadata;

public class MetadataGroupTransferAnalyzer
{
    private QuickTimeToMpeg4VersionsMetadataGroup _beforeTransfer;
    private QuickTimeToMpeg4VersionsMetadataGroup _afterTransfer;

    public MetadataGroupTransferAnalyzer(QuickTimeToMpeg4VersionsMetadataGroup beforeTransfer, QuickTimeToMpeg4VersionsMetadataGroup afterTransfer)
    {
        _beforeTransfer = beforeTransfer;
        _afterTransfer = afterTransfer;
    }

    public IEnumerable<MetadataTransferResult> Analyze()
    {
        var quickTimeMetadataContainer = _beforeTransfer.QuickTimeMetadataContainer;

        var results = new List<MetadataTransferResult>();

        foreach (var mpeg4 in _afterTransfer.Mpeg4MetadataContainers)
        {
            var beforeTransferPair = QuickTimeMpeg4MetadataPair.Create(_beforeTransfer.QuickTimeMetadataContainer.FileInfo.FullName, mpeg4.FileInfo.DirectoryName).Value;
            var afterTransferPair = QuickTimeMpeg4MetadataPair.Create(_afterTransfer.QuickTimeMetadataContainer.FileInfo.FullName, mpeg4.FileInfo.DirectoryName).Value;

            var descriptionTransferred = !beforeTransferPair.IsDescriptionSame && quickTimeMetadataContainer.Description.HasValue
                ? quickTimeMetadataContainer.Description.Value
                : null;

            var yearTransferred = !beforeTransferPair.IsYearSame && quickTimeMetadataContainer.YearByFilename.HasValue
                ? quickTimeMetadataContainer.YearByFilename.Value
                : (uint?)null;

            bool isDescriptionTransferred = descriptionTransferred != null;
            bool isYearTransferred = yearTransferred.HasValue;

            var result = MetadataTransferResult.Create(
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

            results.Add(result);
        }

        return results;
    }
}
