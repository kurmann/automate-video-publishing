using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Verwaltet die Metadaten einer QuickTime-Datei.
/// Die Metadaten werden beim Erzeugen der Instanz gelesen und gespeichert.
/// </summary>
public class QuickTimeMetadataContainer
{
    /// <summary>
    /// Enthält die Rohmetadaten, die aus der Datei gelesen wurden.
    /// </summary>
    public IReadOnlyDictionary<string, string?> RawMetadata { get; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private QuickTimeMetadataContainer(IReadOnlyDictionary<string, string?> rawMetadata)
    {
        RawMetadata = rawMetadata;
    }

    private static Result<IReadOnlyDictionary<string, string?>> TryGetQuickTimeMetadata(FileInfoContainer fileInfoContainer)
    {
        var directories = ImageMetadataReader.ReadMetadata(fileInfoContainer.File.FullName);

        var quickTimeMetadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

        if (quickTimeMetadata != null)
        {
            var metadataDict = quickTimeMetadata.Tags
                .ToDictionary(tag => tag.Name, tag => tag.Description);
            return Result.Success((IReadOnlyDictionary<string, string?>)metadataDict);
        }

        return Result.Failure<IReadOnlyDictionary<string, string?>>("QuickTime metadata not found");
    }

    /// <summary>
    /// Gibt den Namen aus den Metadaten zurück, falls vorhanden, sonst einen leeren String.
    /// </summary>
    public string GetNameOrEmpty()
    {
        if (RawMetadata.TryGetValue("Name", out var name))
        {
            return name ?? string.Empty;
        }

        return string.Empty;
    }

    /// <summary>
    /// Gibt die Beschreibung aus den Metadaten zurück, falls vorhanden, sonst einen leeren String.
    /// </summary>
    public string GetDescriptionOrEmpty()
    {
        if (RawMetadata.TryGetValue("Description", out var description))
        {
            return description ?? string.Empty;
        }

        return string.Empty;
    }

    public static Result<QuickTimeMetadataContainer> Create(string? filePath) => FileInfoContainer.Create(filePath)
            .Bind(fileInfoContainer => TryGetQuickTimeMetadata(fileInfoContainer)
            .Map(quickTimeMetadata => new QuickTimeMetadataContainer(quickTimeMetadata)));

}