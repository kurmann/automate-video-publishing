namespace AutomateVideoPublishing.Entities;

public class ValidMediaLocalDirectory
{
    /// <summary>
    /// Der vollständige Pfad zum Verzeichnis. Beispiel: "C:\MyFolder\MySubFolder"
    /// </summary>
    public string FullPath { get; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private ValidMediaLocalDirectory(string path) => FullPath = path;

    /// <summary>
    /// Erstellt eine ValidMediaLocalDirectory-Instanz aus einem Verzeichnispfad.
    /// Überprüft, ob der bereitgestellte Pfad null oder leer ist und ob das Verzeichnis existiert.
    /// </summary>
    /// <param name="path">Der Pfad, aus dem die ValidMediaLocalDirectory erstellt werden soll.</param>
    /// <returns>Eine Result-Instanz, die entweder eine ValidMediaLocalDirectory-Instanz oder eine Fehlermeldung enthält.</returns>
    public static Result<ValidMediaLocalDirectory> Create(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<ValidMediaLocalDirectory>("Directory path of published local media cannot be null or empty.");
        }

        var directoryInfo = new DirectoryInfo(path);

        if (!directoryInfo.Exists)
        {
            return Result.Failure<ValidMediaLocalDirectory>($"Directory does not exist: {directoryInfo.FullName}");
        }

        return Result.Success(new ValidMediaLocalDirectory(directoryInfo.FullName));
    }
}
