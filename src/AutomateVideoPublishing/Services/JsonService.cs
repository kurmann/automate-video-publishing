using System.Text.Json;

namespace AutomateVideoPublishing.Services;

public static class JsonService
{
    public static string GetFormattedUnicodeJson(object? objectToSerialize)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var json = JsonSerializer.Serialize(objectToSerialize, options);

        return json;
    }
}
