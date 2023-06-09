namespace Services.FileService;

/// <summary>
/// Verantwortlich für das Verwalten von Quell- und Ziel-Dateien für Metadaten-Operationen.
/// Die Quelldatei wird zum Lesen von Metadaten und die Ziel-Datei zum Schreiben von Metadaten verwendet.
/// Stellt eine Factory-Methode bereit, um die Datei-Container-Instanz mit Überprüfung der Dateipfade und -erweiterungen zu erstellen.
/// </summary>
public class SourceAndTargetFileContainer
{
    public FileInfo Source { get; private set; }
    public FileInfo Target { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private SourceAndTargetFileContainer(string sourceFile, string targetFile)
    {
        if (string.IsNullOrWhiteSpace(sourceFile) || string.IsNullOrWhiteSpace(targetFile))
        {
            throw new ArgumentNullException("Error: One or both file paths are null or empty.");
        }

        if (!System.IO.File.Exists(sourceFile) || !System.IO.File.Exists(targetFile))
        {
            throw new ArgumentOutOfRangeException($"Error: One or both files do not exist.");
        }

        string sourceExtension = Path.GetExtension(sourceFile);
        string targetExtension = Path.GetExtension(targetFile);

        if (sourceExtension.ToLower() != ".mov" || targetExtension.ToLower() != ".m4v")
        {
            throw new ArgumentException($"Error: Source file must be a .mov file and target file must be a .m4v file.");
        }

        // Speichern der Dateien als Eigenschaften
        Source = new FileInfo(sourceFile);
        Target = new FileInfo(targetFile);
    }

    /// <summary>
    /// Factory-Methode, die eine neue SourceAndTargetFileContainer-Instanz erstellt und zurückgibt.
    /// Überprüft die Gültigkeit der Quell- und Ziel-Dateipfade und ihre Erweiterungen, bevor die Instanz erstellt wird.
    /// </summary>
    /// <param name="sourceFile"></param>
    /// <param name="targetFile"></param>
    /// <returns></returns>
    public static SourceAndTargetFileContainer Create(string sourceFile, string targetFile)
    {
        return new SourceAndTargetFileContainer(sourceFile, targetFile);
    }
}
