namespace Services.FileService
{
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
    public class FileContainer
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
        private FileContainer(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException("Error on setting file: File path is null or empty.");
            }

            if (!System.IO.File.Exists(filePath))
            {
                throw new ArgumentOutOfRangeException($"Error on setting file: file {filePath} does not exist");
            }

            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".mov")
            {
                // Speichern der Datei als Eigenschaft
                File = new FileInfo(filePath);
                FileType = FileType.QuickTimeMov;
            }
            else if (extension == ".m4v" || extension == ".mp4")
            {
                File = new FileInfo(filePath);
                FileType = FileType.Mpeg4;
            }
            else
            {
                throw new ArgumentException($"Error on setting file: file {filePath} is not a .mov, .m4v or .mp4 file");
            }
        }

        /// <summary>
        /// Factory-Methode, die eine neue FileContainer-Instanz erstellt und zurückgibt.
        /// </summary>
        /// <param name="filePath">Pfad zur Datei, die als Eigenschaft gespeichert werden soll.</param>
        /// <returns>Eine neue Instanz von FileContainer.</returns>
        public static FileContainer Create(string filePath)
        {
            return new FileContainer(filePath);
        }
    }
}
