using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

class Program
{
    static void Main(string[] args)
    {
        var filePath = "";
        var directories = ImageMetadataReader.ReadMetadata(filePath);

        var quickTimeMetadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

        if (quickTimeMetadata != null)
        {
            var descriptionTag = quickTimeMetadata.Tags.FirstOrDefault(t => t.Name == "Description");
            var description = quickTimeMetadata.GetDescription(QuickTimeMetadataHeaderDirectory.TagDescription);
            if (descriptionTag != null)
            {
                Console.WriteLine(descriptionTag.Description);
            }
            else
            {
                Console.WriteLine("Kein 'Description'-Tag gefunden.");
            }
        }
        else
        {
            Console.WriteLine("Keine QuickTime Metadaten gefunden.");
        }
    }
}
