namespace AutomateVideoPublishing.Entities.AppleCompressor;

/// <summary>
/// Repräsentiert gültige Apple Compressor Einstellungen.
/// Diese Klasse stellt sicher, dass der MasterFilePfad existiert und dass der Profilname und das Ausgabeverzeichnis nicht leer sind.
/// </summary>
public class AppleCompressorSettings
{
    /// <summary>
    /// Der Pfad zur Masterdatei.
    /// </summary>
    public FileInfo MasterFile { get; }

    /// <summary>
    /// Das Ausgabeverzeichnis für die komprimierte Datei.
    /// </summary>
    public string OutputDirectory { get; }

    /// <summary>
    /// Der Name des Profils.
    /// </summary>
    public string ProfileName { get; }

    private AppleCompressorSettings(FileInfo masterFile, string outputDirectory, string profileName)
    {
        MasterFile = masterFile;
        OutputDirectory = outputDirectory;
        ProfileName = profileName;
    }

    /// <summary>
    /// Erstellt ein neues AppleCompressorSettings-Objekt.
    /// </summary>
    /// <param name="masterFilePath">Der Pfad zur Masterdatei</param>
    /// <param name="outputDirectory">Das Ausgabeverzeichnis für die komprimierte Datei</param>
    /// <param name="profileName">Der Name des Profils</param>
    /// <returns>Ein Result, das entweder das ValidAppleCompressorSettings-Objekt im Erfolgsfall enthält, oder einen Fehler im Fehlerfall</returns>
    public static Result<AppleCompressorSettings> Create(string masterFilePath, string outputDirectory, string profileName)
    {
        if (string.IsNullOrWhiteSpace(masterFilePath))
        {
            return Result.Failure<AppleCompressorSettings>("Der Pfad zur Masterdatei ist leer oder wurde nicht angegeben.");
        }

        var masterFile = new FileInfo(masterFilePath);
        if (!masterFile.Exists)
        {
            return Result.Failure<AppleCompressorSettings>("Die angegebene Masterdatei existiert nicht.");
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            return Result.Failure<AppleCompressorSettings>("Das Ausgabeverzeichnis ist leer oder wurde nicht angegeben.");
        }

        if (string.IsNullOrWhiteSpace(profileName))
        {
            return Result.Failure<AppleCompressorSettings>("Der Profilname ist leer oder wurde nicht angegeben.");
        }

        return Result.Success(new AppleCompressorSettings(masterFile, outputDirectory, profileName));
    }
}
