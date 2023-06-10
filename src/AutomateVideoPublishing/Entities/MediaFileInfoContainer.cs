using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Enum zur Definition der unterstützten Medien-Dateitypen.
/// </summary>
public enum MediaType
{
    QuickTimeMov,
    Mpeg4
}

/// <summary>
/// Verwaltet eine Mediendatei, die zum Lesen von Metadaten verwendet wird.
/// Die Datei muss eine .mov, .m4v oder .mp4 Datei sein.
/// </summary>
public class MediaFileInfoContainer
{
    /// <summary>
    /// Die Mediendatei, von der Metadaten gelesen werden. 
    /// Es muss eine .mov, .m4v oder .mp4 Datei sein.
    /// </summary>
    public FileInfo File { get; private set; }

    /// <summary>
    /// Der Medien-Typ der verwalteten Datei.
    /// </summary>
    public MediaType MediaType { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private MediaFileInfoContainer(FileInfo file, MediaType mediaType)
    {
        File = file;
        MediaType = mediaType;
    }

    /// <summary>
    /// Factory-Methode, die eine neue MediaFileInfoContainer-Instanz erstellt und zurückgibt.
    /// </summary>
    /// <param name="filePath">Pfad zur Mediendatei, die als Eigenschaft gespeichert werden soll.</param>
    /// <returns>Eine Result-Instanz, die entweder eine MediaFileInfoContainer-Instanz oder eine Fehlermeldung enthält.</returns>
    public static Result<MediaFileInfoContainer> Create(string? filePath)
    {
        if (filePath == null)
        {
            return Result.Failure<MediaFileInfoContainer>("File path is null");
        }

        var file = new FileInfo(filePath);

        if (file == null)
        {
            return Result.Failure<MediaFileInfoContainer>("Error on setting file: File is null.");
        }

        if (!file.Exists)
        {
            return Result.Failure<MediaFileInfoContainer>($"Error on setting file: file {file.FullName} does not exist");
        }

        string extension = file.Extension.ToLower();

        if (extension == ".mov")
        {
            // Erzeugen und Rückgabe einer MediaFileInfoContainer-Instanz mit der Datei und dem Medien-Typ.
            return Result.Success(new MediaFileInfoContainer(file, MediaType.QuickTimeMov));
        }
        else if (extension == ".m4v" || extension == ".mp4")
        {
            return Result.Success(new MediaFileInfoContainer(file, MediaType.Mpeg4));
        }
        else
        {
            return Result.Failure<MediaFileInfoContainer>($"Error on setting file: file {file.FullName} is not a .mov, .m4v or .mp4 file");
        }
    }
}
