using System.Text.Json;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using CommandLine;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => RunCommand(opts));
    }

    static void RunCommand(Options opts)
    {
        if (opts.File == null)
        {
            Console.WriteLine("Datei-Parameter wurde nicht angegeben.");
            return;
        }

        if (opts.ReadMetadata)
        {
            var directories = ImageMetadataReader.ReadMetadata(opts.File);
            var quickTimeMetadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

            if (quickTimeMetadata != null)
            {
                var quickTimeMetadataTags = quickTimeMetadata.Tags
                    .Select(tag => new MetadataTag { Name = tag.Name, Description = tag.Description })
                    .ToList();

                var json = JsonSerializer.Serialize(quickTimeMetadataTags, new JsonSerializerOptions { WriteIndented = true });
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine("Keine QuickTime Metadaten gefunden.");
            }
        }
    }

    public class Options
    {
        [Option('r', "read-metadata", Required = false, HelpText = "Liest Metadaten aus der angegebenen Datei.")]
        public bool ReadMetadata { get; set; }

        [Value(0, MetaName = "file", HelpText = "Dateipfad.", Required = true)]
        public string? File { get; set; }
    }

    public class MetadataTag
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
