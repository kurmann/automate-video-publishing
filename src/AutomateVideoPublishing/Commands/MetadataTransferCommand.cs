using System.Reactive.Linq;
using System.Reactive.Subjects;
using TagLib;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Das Command zum Übertragen der Metadaten von einer Quell- auf eine Zieldatei.
/// </summary>
public class MetadataTransferCommand : ICommand<CombinedMetadataTransferResult>
{
    private readonly PairCollectorCommand _pairCollectorCommand;
    private readonly Subject<CombinedMetadataTransferResult> _broadcaster = new();

    /// <summary>
    /// Observable, das das Ergebnis der Metadatenübertragung liefert.
    /// </summary>
    public IObservable<CombinedMetadataTransferResult> WhenDataAvailable => _broadcaster.AsObservable();

    /// <summary>
    /// Konstruktor, der das PairCollectorCommand als Abhängigkeit erhält.
    /// </summary>
    /// <param name="pairCollectorCommand">Das PairCollectorCommand, das die Paare von QuickTime und MPEG4 Dateien liefert.</param>
    public MetadataTransferCommand(PairCollectorCommand pairCollectorCommand) => _pairCollectorCommand = pairCollectorCommand;

    /// <summary>
    /// Führt das Command aus, indem es die Metadaten von der Quelle auf das Ziel überträgt.
    /// </summary>
    /// <param name="context">Der Kontext des Workflows, in dem das Command ausgeführt wird.</param>
    public void Execute(WorkflowContext context)
    {
        _pairCollectorCommand.WhenDataAvailable
            .Subscribe(pair =>
            {
                // Erstelle ein kombiniertes Transferergebnisobjekt
                var combinedResult = new CombinedMetadataTransferResult(pair, MetadataAttribute.Description | MetadataAttribute.Year);

                // Übertrage die Metadatenänderungen von QuickTime zu MPEG4
                var result = ApplyMetadataChanges(pair);
                if (result.IsSuccess)
                {
                    // Analysiere die durchgeführte Metadatenübertragung
                    var analyzeResults = AnalyzeTransfer(pair);
                    if (analyzeResults.IsFailure)
                    {
                        // Bei Fehler die Fehlermeldung als Ausnahme an den Observer senden
                        _broadcaster.OnError(new Exception(analyzeResults.Error));
                    }
                    else
                    {
                        // Füge die einzelnen Metadatenübertragungsergebnisse dem kombinierten Ergebnis hinzu und sende dieses an den Observer
                        foreach (var analyzeResult in analyzeResults.Value)
                        {
                            combinedResult.AddAttributeResult(analyzeResult);
                        }
                        _broadcaster.OnNext(combinedResult);
                    }
                }
                else
                {
                    // Bei Fehler die Fehlermeldung als Ausnahme an den Observer senden
                    _broadcaster.OnError(new Exception($"Unerwarteter Fehler beim Metadatentransfer: {result.Error}"));
                }
            });

        // Führt das Paar-Sammel-Command aus
        _pairCollectorCommand.Execute(context);

        // Informiert den Observer darüber, dass keine weiteren Daten verfügbar sind
        _broadcaster.OnCompleted();
    }

    /// <summary>
    /// Wendet die Metadatenänderungen auf die Ziel-Datei an.
    /// </summary>
    /// <param name="pair">Das Paar von Quell- und Ziel-Metadaten-Containern.</param>
    /// <returns>Das Ergebnis der Operation.</returns>
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

            // Übertrage die Metadatenänderungen auf die Zieldatei und speichere die Datei
            if (descriptionTransferred != null)
            {
                mpeg4TagLibFile.Tag.Description = descriptionTransferred;
            }

            if (yearTransferred.HasValue)
            {
                mpeg4TagLibFile.Tag.Year = yearTransferred.Value;
            }

            // Setze Apple Tag (todo: wenn vollständig funktioniert)
            // SetReleaseDate(mpeg4TagLibFile, new DateTime(2024, 02, 24));

            mpeg4TagLibFile.Save();
            return Result.Success();
        }
        catch (Exception ex)
        {
            // Bei einem Fehler gibt ein Failure-Ergebnis zurück
            return Result.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Analysiert die durchgeführte Metadatenübertragung und liefert die Ergebnisse der einzelnen Übertragungen zurück.
    /// </summary>
    /// <param name="pair">Das Paar von Quell- und Ziel-Metadaten-Containern.</param>
    /// <returns>Die Liste der Ergebnisse der einzelnen Metadatenübertragungen.</returns>
    private Result<List<MetadataTransferResult>> AnalyzeTransfer(QuickTimeMpeg4MetadataContainerPair pair)
    {
        // Lese Metadaten nach dem die Änderungen angewandt wurden
        var postPair = QuickTimeMpeg4MetadataContainerPair.Create(pair.QuickTimeContainer.FileInfo.FullName,
                                                                  pair.Mpeg4Container.FileInfo.FullName);

        if (postPair.IsFailure)
        {
            return Result.Failure<List<MetadataTransferResult>>($"Unerwarteter Fehler bei der Analyse des ausgeführten Metadatentransfers: {postPair.Error}");
        }

        // Vergleiche die Metadaten vor und nachher.
        var analyzer = MetadataGroupTransferAnalyzer.Create(pair, postPair.Value);
        var metadataAttribute = MetadataAttribute.Description | MetadataAttribute.Year;
        var analyzeResults = analyzer.Analyze(metadataAttribute);

        return Result.Success(analyzeResults);
    }

    static void SetReleaseDate(TagLib.File taglibFile, DateTime releaseDate)
    {
        var tags = (TagLib.Mpeg4.AppleTag)taglibFile.GetTag(TagLib.TagTypes.Apple);

        TagLib.ByteVector releaseDateTagName = new TagLib.ByteVector(new byte[] { 0xA9, (byte)'d', (byte)'a', (byte)'y' });
        string releaseDateText = releaseDate.ToString("yyyy");
        tags.SetText(releaseDateTagName, releaseDateText);
    }




}
