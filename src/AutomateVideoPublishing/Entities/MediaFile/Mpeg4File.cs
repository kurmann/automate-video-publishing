namespace AutomateVideoPublishing.Entities.MediaFile;

/// <summary>
/// Repräsentiert eine gültige MPEG4-Datei.
/// Diese Klasse stellt sicher, dass die angegebene Datei existiert und eine MPEG4-Datei ist (.mp4, .m4v).
/// </summary>
public class Mpeg4File
{
    /// <summary>
    /// Die MPEG4-Datei.
    /// </summary>
    public FileInfo Value { get; }

    private Mpeg4File(FileInfo file) => Value = file;

    /// <summary>
    /// Erstellt ein neues Mpeg4File-Objekt.
    /// </summary>
    /// <param name="filePath">Der Pfad zur Datei</param>
    /// <returns>Ein Result, das entweder das Mpeg4File-Objekt im Erfolgsfall enthält, oder einen Fehler im Fehlerfall</returns>
    public static Result<Mpeg4File> Create(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return Result.Failure<Mpeg4File>("File path cannot be null or whitespace.");
        }

        var file = new FileInfo(filePath);
        if (!file.Exists)
        {
            return Result.Failure<Mpeg4File>("MPEG4 file does not exist.");
        }

        var extension = file.Extension;
        if (!string.Equals(extension, ".mp4", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".MP4", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".m4v", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(extension, ".M4V", StringComparison.OrdinalIgnoreCase))
        {
            return Result.Failure<Mpeg4File>("File is not a MPEG4 file.");
        }

        return Result.Success(new Mpeg4File(file));
    }
}
