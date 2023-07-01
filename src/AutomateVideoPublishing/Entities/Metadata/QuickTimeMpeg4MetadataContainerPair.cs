namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Vereint ein Paar aus QuickTime- und MPEG-4-Dateien einschließlich deren Metadaten in einem einzigen Container.
/// </summary>
public class QuickTimeMpeg4MetadataContainerPair
{
    /// <summary>
    /// Enthält den QuickTimeMetadataContainer, der die Metadaten der QuickTime-Datei darstellt.
    /// </summary>
    public QuickTimeMetadataContainer QuickTimeContainer { get; private set; }

    /// <summary>
    /// Enthält den Mpeg4MetadataContainer, der die Metadaten der MPEG-4-Datei darstellt.
    /// </summary>
    public Mpeg4MetadataContainer Mpeg4Container { get; private set; }

    public bool IsYearSame
    {
        get
        {
            var sourceYear = QuickTimeContainer.YearByFilename;
            var targetYear = Mpeg4Container.Year;

            if (sourceYear.HasValue && targetYear.HasValue)
            {
                return sourceYear.Value == targetYear.Value;
            }

            return false;
        }
    }

    public bool IsDescriptionSame
    {
        get
        {
            var sourceDescription = QuickTimeContainer.Description;
            var targetDescription = Mpeg4Container.Description;

            if (sourceDescription.HasValue && targetDescription.HasValue)
            {
                return sourceDescription.Value == targetDescription;
            }

            return false;
        }
    }

    private QuickTimeMpeg4MetadataContainerPair(QuickTimeMetadataContainer quickTimeContainer, Mpeg4MetadataContainer mpeg4Container)
    {
        QuickTimeContainer = quickTimeContainer;
        Mpeg4Container = mpeg4Container;
    }

    /// <summary>
    /// Erstellt eine neue Instanz der QuickTimeMpeg4MetadataContainerPair-Klasse basierend auf den Pfaden der Input-Dateien.
    /// </summary>
    /// <param name="quickTimeFilePath">Der Pfad zur QuickTime-Datei.</param>
    /// <param name="mpeg4FilePath">Der Pfad zur MPEG-4-Datei.</param>
    /// <returns>Ein Ergebnisobjekt, das entweder das erstellte Paar oder einen Fehler enthält.</returns>
    public static Result<QuickTimeMpeg4MetadataContainerPair> Create(string quickTimeFilePath, string mpeg4FilePath)
    {
        var quickTimeResult = QuickTimeMetadataContainer.Create(quickTimeFilePath);
        if (quickTimeResult.IsFailure)
        {
            return Result.Failure<QuickTimeMpeg4MetadataContainerPair>(quickTimeResult.Error);
        }

        var mpeg4Result = Mpeg4MetadataContainer.Create(mpeg4FilePath);
        if (mpeg4Result.IsFailure)
        {
            return Result.Failure<QuickTimeMpeg4MetadataContainerPair>(mpeg4Result.Error);
        }

        return Result.Success(new QuickTimeMpeg4MetadataContainerPair(quickTimeResult.Value, mpeg4Result.Value));
    }

    /// <summary>
    /// Überschreibt die ToString-Methode, um den Dateinamen des QuickTime- und des MPEG-4-Files, getrennt durch eine Pipe, zurückzugeben.
    /// </summary>
    /// <returns>Eine Zeichenkette, die die QuickTime- und MPEG-4-Dateinamen enthält, getrennt durch eine Pipe.</returns>
    public override string ToString()
    {
        var quickTimeFileName = QuickTimeContainer.FileInfo.Name;
        var mpeg4FileName = Mpeg4Container.FileInfo.Name;

        return $"{quickTimeFileName} | {mpeg4FileName}";
    }

}