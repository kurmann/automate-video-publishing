using YamlDotNet.Serialization;
using YamlDotNet.Core;

namespace AutomateVideoPublishing.Entities.Elementary;

/// <summary>
/// Repräsentiert eine serialisierte YAML-Struktur.
/// </summary>
public class YamlContent
{
    public string Value { get; }

    private YamlContent(string yamlContent) => Value = yamlContent;

    public static Result<YamlContent> CreateFromMetadataSections(object objectToSerialize)
    {
        try
        {
            var serializer = new SerializerBuilder().Build();
            var yaml = serializer.Serialize(objectToSerialize);
            
            return Result.Success(new YamlContent(yaml));
        }
        catch (YamlException ex)
        {
            return Result.Failure<YamlContent>($"Failed to convert to YAML: {ex.Message}");
        }
    }

}
