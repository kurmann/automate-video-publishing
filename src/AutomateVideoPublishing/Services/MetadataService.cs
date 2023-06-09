using AutomateVideoPublishing.Models;
using MetadataExtractor;
using MetadataExtractor.Formats.QuickTime;
using TagLib;

namespace AutomateVideoPublishing.Services;

public class MetadataService
{
    private string m_file;

    public MetadataService(string file, string targetFile = "")
    {
        if (string.IsNullOrEmpty(file))
        {
            throw new ArgumentOutOfRangeException("Cannot init MetadataService due to missing file path");
        }
        m_file = file;
    }

    public List<MetadataTag> TryGetQuickTimeMetadata()
    {
        var directories = ImageMetadataReader.ReadMetadata(m_file);
 
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

    public void SetMp4Metadata(string description)
    {

    }
}