using System.Text.Json;
using System.Text.Json.Serialization;
using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

public class FormattedUnicodeJson
{
    public string Value { get; }

    private FormattedUnicodeJson(string value) => Value = value;

    public static Result<FormattedUnicodeJson> Create(object? objectToSerialize)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            var json = JsonSerializer.Serialize(objectToSerialize, options);

            return Result.Success(new FormattedUnicodeJson(json));
        }
        catch (Exception e)
        {
            return Result.Failure<FormattedUnicodeJson>($"Error when trying to create formatted unicode JSON: {e.Message}");
        }
    }

    public override string ToString() => Value;
}
