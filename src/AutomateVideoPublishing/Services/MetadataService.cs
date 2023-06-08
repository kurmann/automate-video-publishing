using System.Text.Json;
using AutomateVideoPublishing.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

namespace AutomateVideoPublishing.Services;

public class MetadataService {

    public void ReadQuicktimeMetadata(Options opts)
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