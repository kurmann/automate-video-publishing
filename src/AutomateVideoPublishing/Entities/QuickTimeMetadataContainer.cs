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
    private QuickTimeMetadataContainer(IReadOnlyDictionary<string, string?> rawMetadata) => RawMetadata = rawMetadata;

    private static Result<IReadOnlyDictionary<string, string?>> TryGetQuickTimeMetadata(MediaFileInfoContainer fileContainer)
    {
        var directories = ImageMetadataReader.ReadMetadata(fileContainer.File.FullName);

        var metadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

        if (metadata != null)
        {
            var metadataDict = metadata.Tags.ToDictionary(tag => tag.Name, tag => tag.Description);
            return Result.Success((IReadOnlyDictionary<string, string?>)metadataDict);
        }

        return Result.Failure<IReadOnlyDictionary<string, string?>>("QuickTime metadata not found");
    }

    public static Result<QuickTimeMetadataContainer> Create(string? filePath) => MediaFileInfoContainer.Create(filePath)
        .Bind(fileInfoContainer => TryGetQuickTimeMetadata(fileInfoContainer)
        .Map(metadata => new QuickTimeMetadataContainer(metadata)));

    public static Result<QuickTimeMetadataContainer> Create(MediaFileInfoContainer mediaFileInfoContainer) => TryGetQuickTimeMetadata(mediaFileInfoContainer)
        .Map(metadata => new QuickTimeMetadataContainer(metadata));
}