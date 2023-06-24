namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Repräsentiert ein gültiges MPEG-4 Verzeichnis.
/// Diese Klasse stellt sicher, dass das angegebene Verzeichnis existiert und enthält eine Sammlung von MPEG-4 Dateien (.mp4 oder .m4v) innerhalb dieses Verzeichnisses.
/// </summary>
public class ValidMpeg4Directory
{
    /// <summary>
    /// Das Verzeichnis
    /// </summary>
    public DirectoryInfo Directory { get; }

    /// <summary>
    /// Eine Sammlung von FileInfo-Objekten, die MPEG-4 Dateien im Verzeichnis repräsentieren. Wenn keine Dateien gefunden werden, ist die Liste leer.
    /// </summary>
    public IEnumerable<FileInfo> Mpeg4Files { get; }

    private ValidMpeg4Directory(DirectoryInfo directory, List<FileInfo> mpeg4Files)
    {
        Directory = directory;
        Mpeg4Files = mpeg4Files;
    }

    /// <summary>
    /// Erstellt ein neues ValidMpeg4Directory-Objekt.
    /// </summary>
    /// <param name="directoryPath">Der Pfad zum Verzeichnis</param>
    /// <returns>Ein Result, das entweder das ValidMpeg4Directory-Objekt im Erfolgsfall enthält, oder einen Fehler im Fehlerfall</returns>
    public static Result<ValidMpeg4Directory> Create(string? directoryPath)
    {
        directoryPath = string.IsNullOrWhiteSpace(directoryPath) ? "." : directoryPath;

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
        {
            return Result.Failure<ValidMpeg4Directory>("MPEG-4 directory does not exist.");
        }

        var mpeg4Files = directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
            .Where(f => string.Equals(f.Extension, ".mp4", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(f.Extension, ".m4v", StringComparison.OrdinalIgnoreCase)).ToList();

        return Result.Success(new ValidMpeg4Directory(directory, mpeg4Files));
    }
}