namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Repräsentiert ein gültiges QuickTime-Master-Verzeichnis.
/// Diese Klasse stellt sicher, dass das angegebene Verzeichnis existiert und enthält eine Sammlung von QuickTime-Dateien (.mov) innerhalb dieses Verzeichnisses.
/// </summary>
public class ValidQuickTimeMasterDirectory
{
    /// <summary>
    /// Das Verzeichnis
    /// </summary>
    public DirectoryInfo Directory { get; }

    /// <summary>
    /// Eine Sammlung von FileInfo-Objekten, die QuickTime-Dateien im Verzeichnis repräsentieren. Wenn keine Dateien gefunden werden, ist die Liste leer.
    /// </summary>
    public IReadOnlyList<FileInfo> QuickTimeFiles { get; }

    private ValidQuickTimeMasterDirectory(DirectoryInfo directory, List<FileInfo> quickTimeFiles)
    {
        Directory = directory;
        QuickTimeFiles = quickTimeFiles;
    }

    /// <summary>
    /// Erstellt ein neues ValidQuickTimeMasterDirectory-Objekt.
    /// </summary>
    /// <param name="directoryPath">Der Pfad zum Verzeichnis</param>
    /// <returns>Ein Result, das entweder das ValidQuickTimeMasterDirectory-Objekt im Erfolgsfall enthält, oder einen Fehler im Fehlerfall</returns>
    public static Result<ValidQuickTimeMasterDirectory> Create(string? directoryPath)
    {
        directoryPath = string.IsNullOrWhiteSpace(directoryPath) ? "." : directoryPath;

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
        {
            return Result.Failure<ValidQuickTimeMasterDirectory>($"Directory {directoryPath} does not exist.");
        }

        var quickTimeFiles = directory.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
            .Where(f => string.Equals(f.Extension, ".mov", StringComparison.OrdinalIgnoreCase)).ToList();

        return Result.Success(new ValidQuickTimeMasterDirectory(directory, quickTimeFiles));
    }
}