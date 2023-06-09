using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using Services.FileService;
using CSharpFunctionalExtensions;

namespace Services.MetadataService
{
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

        private static Result<IReadOnlyDictionary<string, string?>> TryGetQuickTimeMetadata(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);

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

        /// <summary>
        /// Factory-Methode, die eine neue QuickTimeMetadataContainer-Instanz erstellt und zurückgibt.
        /// </summary>
        /// <param name="fileContainer">Der FileContainer, der die zu analysierende Datei enthält.</param>
        /// <returns>Ein Result, das entweder eine neue Instanz von QuickTimeMetadataContainer enthält oder einen Fehler.</returns>
        public static Result<QuickTimeMetadataContainer> Create(FileContainer fileContainer)
        {
            if (fileContainer == null)
            {
                return Result.Failure<QuickTimeMetadataContainer>("FileContainer is null");
            }

            if (fileContainer.FileType != FileType.Mpeg4)
            {
                return Result.Failure<QuickTimeMetadataContainer>("FileContainer does not reference a MP4 file");
            }

            var metadataResult = TryGetQuickTimeMetadata(fileContainer.File.FullName);

            if (metadataResult.IsFailure)
            {
                return Result.Failure<QuickTimeMetadataContainer>(metadataResult.Error);
            }

            return Result.Success(new QuickTimeMetadataContainer(metadataResult.Value));
        }
    }
}