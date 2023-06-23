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
                var result = ApplyMetadataChanges(pair!);
                if (result.IsSuccess)
                {
                    var analyzeResult = AnalyzeTransfer(pair!);
                    if (analyzeResult.IsFailure)
                    {
                        _broadcaster.OnError(new Exception(analyzeResult.Error));
                    }
                    else 
                    {
                        _broadcaster.OnNext(analyzeResult.Value);
                    }
                }
                else
                {
                    _broadcaster.OnError(new Exception($"Unexpected error on metadata transfer: {result.Error}"));
                }
            });

        _quickTimeCommand.Execute(context);
        _broadcaster.OnCompleted();
    }

    private Result ApplyMetadataChanges(QuickTimeMpeg4MetadataPair pair)
    {
        try
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

            if (descriptionTransferred != null)
            {
                mpeg4TagLibFile.Tag.Description = descriptionTransferred;
            }

            if (yearTransferred.HasValue)
            {
                mpeg4TagLibFile.Tag.Year = yearTransferred.Value;
            }

            mpeg4TagLibFile.Save();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }

    private Result<MetadataTransferResult> AnalyzeTransfer(QuickTimeMpeg4MetadataPair pair)
    {
        // Read metadata after changes are applied.
        var postPair = QuickTimeMpeg4MetadataPair.Create(pair.QuickTimeMetadataContainer.FileInfo.FullName, pair.Mpeg4MetadataContainer.FileInfo.DirectoryName);

        if (postPair.IsFailure)
        {
            return Result.Failure<MetadataTransferResult>($"Unexpected error on analizying executed metadata transfer: {postPair.Error}");
        }

        var analyzer = new MetadataTransferAnalyzer(pair, postPair.Value);
        return analyzer.Analyze();
    }


}
