using System.Collections.ObjectModel;
using System.Text;

namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Enthält alle Metadatentransferergebnisse für eine Paarung von QuickTime- und Mpeg4-Dateien.
/// </summary>
public class CombinedMetadataTransferResult
{
    /// <summary>
    /// Enthält das Paar QuickTime- und Mpeg4-Metadaten-Container.
    /// </summary>
    public QuickTimeMpeg4MetadataContainerPair MetadataContainerPair { get; }

    /// <summary>
    /// Enthält das Metadatenattribut, das transferiert werden soll.
    /// </summary>
    public MetadataAttribute MetadataAttributes { get; }

    private readonly List<MetadataTransferResult> _transferResults = new();

    /// <summary>
    /// Erstellt eine Instanz von CombinedMetadataTransferResult.
    /// </summary>
    /// <param name="metadataContainerPair">Das Paar von QuickTime- und Mpeg4-Metadaten-Container.</param>
    /// <param name="metadataAttributes">Die Metadatenattribute, die transferiert werden sollen.</param>
    public CombinedMetadataTransferResult(QuickTimeMpeg4MetadataContainerPair metadataContainerPair, MetadataAttribute metadataAttributes)
    {
        MetadataContainerPair = metadataContainerPair;
        MetadataAttributes = metadataAttributes;
    }

    /// <summary>
    /// Fügt ein Metadatentransferergebnis hinzu.
    /// </summary>
    /// <param name="result">Das Metadatentransferergebnis.</param>
    public void AddAttributeResult(MetadataTransferResult result) => _transferResults.Add(result);

    /// <summary>
    /// Gibt eine schreibgeschützte Liste aller Metadatentransferergebnisse zurück.
    /// </summary>
    public ReadOnlyCollection<MetadataTransferResult> TransferResults => _transferResults.AsReadOnly();

    /// <summary>
    /// Überprüft, ob alle Metadatentransfers erfolgreich waren.
    /// </summary>
    public bool IsAllTransferSuccess => _transferResults.All(result => result.Status == TransferStatus.Success);

    public string SummaryMessage
    {
        get
        {
            var messageBuilder = new StringBuilder();
            string quickTimeFile = MetadataContainerPair.QuickTimeContainer.FileInfo.FullName;
            string mpeg4File = MetadataContainerPair.Mpeg4Container.FileInfo.FullName;
            messageBuilder.AppendLine($"Metadata transfer summary for file: {quickTimeFile} to file: {mpeg4File}.");

            // Hinzufügen der Zusammenfassungsnachrichten für jedes Transferergebnis
            foreach (var result in TransferResults)
            {
                messageBuilder.AppendLine(result.SummaryMessage);
            }

            // Hinzufügen einer abschließenden Zusammenfassung, ob alle Transfers erfolgreich waren
            messageBuilder.AppendLine(IsAllTransferSuccess
                ? "All metadata transfers were successful."
                : "Some metadata transfers failed.");

            return messageBuilder.ToString();
        }
    }

}
