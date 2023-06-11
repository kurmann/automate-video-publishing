using CSharpFunctionalExtensions;
/// <summary>
/// Ableitung von FileInfo mit eine Auswahl für diese Anwendung relevanten Eigenschaften.
/// Die Klasse ist im Gegensatz FileInfo serialisierbar.
/// </summary>
public class SerializableFileInfo
{
    /// <summary>
    /// Der Dateiname inklusive der Erweiterung. Beispiel: "MyFile.txt"
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Die Dateierweiterung inklusive des Punkts. Beispiel: ".txt"
    /// </summary>
    public string Extension { get; set; }

    /// <summary>
    /// Die Länge der Datei in Bytes. Dies ist eine Längenangabe und kein Maß für die physische Größe der Datei auf dem Datenträger.
    /// </summary>
    public long Length { get; set; }

    /// <summary>
    /// Der vollständige Pfad zur Datei, inklusive des Dateinamens. Beispiel: "C:\MyFolder\MyFile.txt"
    /// </summary>
    public string FullPath { get; set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private SerializableFileInfo(FileInfo fileInfo)
    {
        FileName = fileInfo.Name;
        Extension = fileInfo.Extension;
        Length = fileInfo.Length;
        FullPath = fileInfo.FullName;
    }

    /// <summary>
    /// Erstellt eine SerializableFileInfo-Instanz aus einer FileInfo-Instanz.
    /// Überprüft, ob die bereitgestellte FileInfo-Instanz null ist oder ob die Datei existiert.
    /// </summary>
    /// <param name="fileInfo">Die FileInfo-Instanz, aus der die SerializableFileInfo erstellt werden soll.</param>
    /// <returns>Eine Result-Instanz, die entweder eine SerializableFileInfo-Instanz oder eine Fehlermeldung enthält.</returns>
    public static Result<SerializableFileInfo> FromFileInfo(FileInfo fileInfo)
    {
        if (fileInfo == null)
        {
            return Result.Failure<SerializableFileInfo>("FileInfo instance cannot be null.");
        }

        if (!fileInfo.Exists)
        {
            return Result.Failure<SerializableFileInfo>($"File does not exist: {fileInfo.FullName}");
        }

        return Result.Success(new SerializableFileInfo(fileInfo));
    }
}
