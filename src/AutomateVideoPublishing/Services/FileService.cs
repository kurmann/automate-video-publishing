namespace Services.FileService;

/// <summary>
/// Ist verantwortlich für das Verwalten von Quelldatei zum Lesen von Metadaten als auch der Zieldatei zum Schreiben von Metadaten.
/// Darüber hinaus bietet der Service Prüfroutinen an.
/// </summary>
public class MediaFileContainer
{
    public FileInfo Mov { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private MediaFileContainer(string file)
    {
        if (string.IsNullOrWhiteSpace(file))
        {
            throw new ArgumentNullException("Error on setting file: File path is null or empty.");
        }

        if (!System.IO.File.Exists(file))
        {
            throw new ArgumentOutOfRangeException($"Error on setting file: file {file} does not exist");
        }

        string extension = Path.GetExtension(file);

        if (extension.ToLower() == ".mov")
        {
            // Speichern der Datei als Eigenschaft
            Mov = new FileInfo(file);
        }
        else
        {
            throw new ArgumentException($"Error on setting file: file {file} is not a .mov file");
        }
    }

    /// <summary>
    /// Factory-Methode, die eine neue FileService-Instanz erstellt und zurückgibt.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static MediaFileContainer Create(string file)
    {
        return new MediaFileContainer(file);
    }
}
