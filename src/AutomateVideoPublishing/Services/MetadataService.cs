using AutomateVideoPublishing.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;

namespace AutomateVideoPublishing.Services;

public class MetadataService
{
    public List<MetadataTag> TryGetQuickTimeMetadata(string file)
    {
        var directories = ImageMetadataReader.ReadMetadata(file);
 
        var quickTimeMetadata = directories.OfType<QuickTimeMetadataHeaderDirectory>().FirstOrDefault();

        if (quickTimeMetadata != null)
        {
            var quickTimeMetadataTags = quickTimeMetadata.Tags
                .Select(tag => new MetadataTag { Name = tag.Name, Description = tag.Description })
                .ToList();

            return quickTimeMetadataTags;
        }
        return new List<MetadataTag>();
    }
}