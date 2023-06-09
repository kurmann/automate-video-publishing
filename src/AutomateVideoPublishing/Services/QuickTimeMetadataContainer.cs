using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using Services.FileService;

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
        private QuickTimeMetadataContainer(FileContainer fileContainer)
        {
            if (fileContainer == null)
            {
                throw new ArgumentNullException("Error: FileContainer is null.");
            }

            if (fileContainer.FileType != FileType.Mpeg4)
            {
                throw new ArgumentException("Error: FileContainer does not reference a MPEG-4 (.mv4 / .mp4) file.");
            }

            RawMetadata = TryGetQuickTimeMetadata(fileContainer.File.FullName);
        }

        private Dictionary<string, string?> TryGetQuickTimeMetadata(string filePath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath);

            var quickTimeMetadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

            if (quickTimeMetadata != null)
            {
                return quickTimeMetadata.Tags
                    .ToDictionary(tag => tag.Name, tag => tag.Description);
            }

            return new Dictionary<string, string?>();
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
        /// <returns>Eine neue Instanz von QuickTimeMetadataContainer.</returns>
        public static QuickTimeMetadataContainer Create(FileContainer fileContainer) => new QuickTimeMetadataContainer(fileContainer);
    }
}
