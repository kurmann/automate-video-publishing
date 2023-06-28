using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class MetadataTransferCommand : ICommand<CombinedMetadataTransferResult>
{
    private readonly PairCollectorCommand _pairCollectorCommand;
    private readonly Subject<CombinedMetadataTransferResult> _broadcaster = new();

    public IObservable<CombinedMetadataTransferResult> WhenDataAvailable => _broadcaster.AsObservable();

    public MetadataTransferCommand(PairCollectorCommand pairCollectorCommand) => _pairCollectorCommand = pairCollectorCommand;

    public void Execute(WorkflowContext context)
    {
        _pairCollectorCommand.WhenDataAvailable
            .Subscribe(pair =>
            {
                var combinedResult = new CombinedMetadataTransferResult(pair, MetadataAttribute.Description | MetadataAttribute.Year);
                var result = ApplyMetadataChanges(pair);
                if (result.IsSuccess)
                {
                    var analyzeResults = AnalyzeTransfer(pair);
                    if (analyzeResults.IsFailure)
                    {
                        _broadcaster.OnError(new Exception(analyzeResults.Error));
                    }
                    else
                    {
                        foreach (var analyzeResult in analyzeResults.Value)
                        {
                            combinedResult.AddAttributeResult(analyzeResult);
                        }
                        _broadcaster.OnNext(combinedResult);
                    }
                }
                else
                {
                    _broadcaster.OnError(new Exception($"Unerwarteter Fehler beim Metadatentransfer: {result.Error}"));
                }
            });

        _pairCollectorCommand.Execute(context);
        _broadcaster.OnCompleted();
    }

    private Result ApplyMetadataChanges(QuickTimeMpeg4MetadataContainerPair pair)
    {
        try
        {
            var quickTimeMetadataContainer = pair.QuickTimeContainer;
            var mpeg4 = pair.Mpeg4Container;

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

    private Result<List<MetadataTransferResult>> AnalyzeTransfer(QuickTimeMpeg4MetadataContainerPair pair)
    {
        // Lese Metadaten nach dem die Änderungen angewandt wurden
        var postPair = QuickTimeMpeg4MetadataContainerPair.Create(pair.QuickTimeContainer.FileInfo.FullName,
                                                                  pair.Mpeg4Container.FileInfo.FullName);

        if (postPair.IsFailure)
        {
            return Result.Failure<List<MetadataTransferResult>>($"Unexpected error on analyzing executed metadata transfer: {postPair.Error}");
        }

        // Vergleiche die Metadaten vor und nachher.
        var analyzer = MetadataGroupTransferAnalyzer.Create(pair, postPair.Value);
        var metadataAttribute = MetadataAttribute.Description | MetadataAttribute.Year;
        var analyzeResults = analyzer.Analyze(metadataAttribute);

        return Result.Success(analyzeResults);
    }
}