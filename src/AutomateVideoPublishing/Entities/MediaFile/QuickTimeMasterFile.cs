namespace AutomateVideoPublishing.Entities.MediaFile;

/// <summary>
/// Repräsentiert eine gültige QuickTime Masterdatei.
/// Diese Klasse stellt sicher, dass die angegebene Datei existiert und eine QuickTime-Datei ist (.mov).
/// </summary>
public class QuickTimeMasterFile
{
    /// <summary>
    /// Die QuickTime-Datei.
    /// </summary>
    public FileInfo Value { get; }

    private QuickTimeMasterFile(FileInfo file) => Value = file;

    /// <summary>
    /// Erstellt ein neues QuickTimeMasterFile-Objekt.
    /// </summary>
    /// <param name="filePath">Der Pfad zur Datei</param>
    /// <returns>Ein Result, das entweder das QuickTimeMasterFile-Objekt im Erfolgsfall enthält, oder einen Fehler im Fehlerfall</returns>
    public static Result<QuickTimeMasterFile> Create(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Failure<QuickTimeMasterFile>("File path cannot be null or whitespace.");
        }

        var file = new FileInfo(filePath);
        if (!file.Exists)
        {
            return Result.Failure<QuickTimeMasterFile>("QuickTime masterfile does not exist.");
        }

        var extension = file.Extension;
        if (!string.Equals(extension, ".mov", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".MOV", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<QuickTimeMasterFile>("File is not a QuickTime file.");
        }

        return Result.Success(new QuickTimeMasterFile(file));
    }
}
