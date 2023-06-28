namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Klasse für die Analyse des Metadaten-Transfers zwischen QuickTime- und Mpeg4-Containern.
/// </summary>
public class MetadataGroupTransferAnalyzer
{
    private QuickTimeMpeg4MetadataContainerPair _beforeTransfer;
    private QuickTimeMpeg4MetadataContainerPair _afterTransfer;

    /// <summary>
    /// Privater Konstruktor, der die Zustände vor und nach dem Transfer initialisiert.
    /// </summary>
    /// <param name="beforeTransfer">Paar von Containern vor dem Transfer.</param>
    /// <param name="afterTransfer">Paar von Containern nach dem Transfer.</param>
    private MetadataGroupTransferAnalyzer(QuickTimeMpeg4MetadataContainerPair beforeTransfer, QuickTimeMpeg4MetadataContainerPair afterTransfer)
    {
        _beforeTransfer = beforeTransfer;
        _afterTransfer = afterTransfer;
    }

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse mit den gegebenen Zuständen vor und nach dem Transfer.
    /// </summary>
    /// <param name="beforeTransfer">Paar von Containern vor dem Transfer.</param>
    /// <param name="afterTransfer">Paar von Containern nach dem Transfer.</param>
    /// <returns>Eine neue Instanz der Klasse.</returns>
    public static MetadataGroupTransferAnalyzer Create(QuickTimeMpeg4MetadataContainerPair beforeTransfer, QuickTimeMpeg4MetadataContainerPair afterTransfer)
    {
        return new MetadataGroupTransferAnalyzer(beforeTransfer, afterTransfer);
    }

    /// <summary>
    /// Führt eine Analyse des Metadaten-Transfers durch und gibt die Ergebnisse zurück.
    /// </summary>
    /// <param name="metadataAttribute">Die zu übertragenden Metadatenattribute.</param>
    /// <returns>Eine Liste der Transferergebnisse für die verschiedenen Metadatenattribute.</returns>
    public List<MetadataTransferResult> Analyze(MetadataAttribute metadataAttribute)
    {
        var results = new List<MetadataTransferResult>();

        var beforeTransferPair = _beforeTransfer;
        var afterTransferPair = _afterTransfer;

        // Überprüfung und Analyse des Beschreibungstransfers, wenn das Attribut gesetzt ist
        if (metadataAttribute.HasFlag(MetadataAttribute.Description)) {
            var descriptionTransferStatus = beforeTransferPair.IsDescriptionSame && afterTransferPair.IsDescriptionSame
                ? TransferStatus.Success
                : TransferStatus.Failed;
            results.Add(
                new MetadataTransferResult(beforeTransferPair.QuickTimeContainer.FileInfo, afterTransferPair.Mpeg4Container.FileInfo, MetadataAttribute.Description, descriptionTransferStatus)
            );
        }

        // Überprüfung und Analyse des Jahrtransfers, wenn das Attribut gesetzt ist
        if(metadataAttribute.HasFlag(MetadataAttribute.Year)) {
            var yearTransferStatus = beforeTransferPair.IsYearSame && afterTransferPair.IsYearSame
                ? TransferStatus.Success
                : TransferStatus.Failed;
            results.Add(
                new MetadataTransferResult(beforeTransferPair.QuickTimeContainer.FileInfo, afterTransferPair.Mpeg4Container.FileInfo, MetadataAttribute.Year, yearTransferStatus)
            );
        }

        return results;
    }
}
