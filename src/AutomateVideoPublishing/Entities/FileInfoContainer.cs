using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Enum zur Definition der unterstützten Dateitypen.
/// </summary>
public enum FileType
{
    QuickTimeMov,
    Mpeg4
}

/// <summary>
/// Verwaltet eine Datei, die zum Lesen von Metadaten verwendet wird.
/// Die Datei muss eine .mov, .m4v oder .mp4 Datei sein.
/// </summary>
public class FileInfoContainer
{
    /// <summary>
    /// Die Datei, von der Metadaten gelesen werden. 
    /// Es muss eine .mov, .m4v oder .mp4 Datei sein.
    /// </summary>
    public FileInfo File { get; private set; }

    /// <summary>
    /// Der Dateityp der verwalteten Datei.
    /// </summary>
    public FileType FileType { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private FileInfoContainer(FileInfo file, FileType fileType)
    {
        File = file;
        FileType = fileType;
    }

    /// <summary>
    /// Factory-Methode, die eine neue FileInfoContainer-Instanz erstellt und zurückgibt.
    /// </summary>
    /// <param name="filePath">Pfad zur Datei, die als Eigenschaft gespeichert werden soll.</param>
    /// <returns>Eine Result-Instanz, die entweder eine FileInfoContainer-Instanz oder eine Fehlermeldung enthält.</returns>
    public static Result<FileInfoContainer> Create(string? filePath)
    {
        if (filePath == null)
        {
            return Result.Failure<FileInfoContainer>("File path is null");
        }

        var file = new FileInfo(filePath);

        if (file == null)
        {
            return Result.Failure<FileInfoContainer>("Error on setting file: File is null.");
        }

        if (!file.Exists)
        {
            return Result.Failure<FileInfoContainer>($"Error on setting file: file {file.FullName} does not exist");
        }

        string extension = file.Extension.ToLower();

        if (extension == ".mov")
        {
            // Erzeugen und Rückgabe einer FileInfoContainer-Instanz mit der Datei und dem Dateityp.
            return Result.Success(new FileInfoContainer(file, FileType.QuickTimeMov));
        }
        else if (extension == ".m4v" || extension == ".mp4")
        {
            return Result.Success(new FileInfoContainer(file, FileType.Mpeg4));
        }
        else
        {
            return Result.Failure<FileInfoContainer>($"Error on setting file: file {file.FullName} is not a .mov, .m4v or .mp4 file");
        }
    }
}
