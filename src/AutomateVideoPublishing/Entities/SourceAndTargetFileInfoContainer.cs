using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Verantwortlich für das Verwalten von Quell- und Ziel-Datei-Informationen für Metadaten-Operationen.
/// Die Quelldatei wird zum Lesen von Metadaten und die Ziel-Datei zum Schreiben von Metadaten verwendet.
/// Stellt eine Factory-Methode bereit, um die Datei-Container-Instanz mit Überprüfung der Dateipfade und -erweiterungen zu erstellen.
/// </summary>
public class SourceAndTargetFileInfoContainer
{
    /// <summary>
    /// Die Quelldatei, von der Metadaten gelesen werden. 
    /// Es muss eine .mov Datei sein.
    /// </summary>
    public FileInfo Source { get; private set; }

    /// <summary>
    /// Die Ziel-Datei, in die Metadaten geschrieben werden.
    /// Es muss eine .m4v oder .mp4 Datei sein.
    /// </summary>
    public FileInfo Target { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private SourceAndTargetFileInfoContainer(FileInfo sourceFile, FileInfo targetFile)
    {
        Source = sourceFile;
        Target = targetFile;
    }

    /// <summary>
    /// Factory-Methode, die eine neue SourceAndTargetFileInfoContainer-Instanz erstellt und zurückgibt.
    /// Überprüft die Gültigkeit der Quell- und Ziel-Dateipfade und ihre Erweiterungen, bevor die Instanz erstellt wird.
    /// </summary>
    /// <param name="sourceFile"></param>
    /// <param name="targetFile"></param>
    /// <returns></returns>
    public static Result<SourceAndTargetFileInfoContainer> Create(string sourceFile, string targetFile)
    {
        if (string.IsNullOrWhiteSpace(sourceFile) || string.IsNullOrWhiteSpace(targetFile))
        {
            return Result.Failure<SourceAndTargetFileInfoContainer>("Error: One or both file paths are null or empty.");
        }

        if (!System.IO.File.Exists(sourceFile) || !System.IO.File.Exists(targetFile))
        {
            return Result.Failure<SourceAndTargetFileInfoContainer>($"Error: One or both files do not exist.");
        }

        string sourceExtension = Path.GetExtension(sourceFile);
        string targetExtension = Path.GetExtension(targetFile).ToLower();

        if (sourceExtension.ToLower() != ".mov" || (targetExtension != ".m4v" && targetExtension != ".mp4"))
        {
            return Result.Failure<SourceAndTargetFileInfoContainer>($"Error: Source file must be a .mov file and target file must be a .m4v or .mp4 file.");
        }

        // Erstellen und Zurückgeben der Datei-Container-Instanz
        return Result.Success(new SourceAndTargetFileInfoContainer(new FileInfo(sourceFile), new FileInfo(targetFile)));
    }
}
