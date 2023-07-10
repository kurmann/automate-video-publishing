using YamlDotNet.Serialization;
using YamlDotNet.Core;

namespace AutomateVideoPublishing.Entities.Metadata;

/// <summary>
/// Repräsentiert eine serialisierte YAML-Struktur.
/// </summary>
public class MediaInfoMetadataYaml
{
    public string YamlContent { get; }

    private MediaInfoMetadataYaml(string yamlContent) => YamlContent = yamlContent;

    public static Result<MediaInfoMetadataYaml> CreateFromMetadataSections(MetadataSections metadataSections)
    {
        try
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(metadataSections);
            
            return Result.Success(new MediaInfoMetadataYaml(yaml));
        }
        catch (YamlException ex)
        {
            return Result.Failure<MediaInfoMetadataYaml>($"Failed to convert sections to YAML: {ex.Message}");
        }
    }
}
