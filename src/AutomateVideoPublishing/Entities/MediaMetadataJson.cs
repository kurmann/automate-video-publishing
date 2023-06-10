using CSharpFunctionalExtensions;

namespace AutomateVideoPublishing.Entities;

/// <summary>
/// Verwaltet die Metadaten einer Medien-Datei im JSON-Format.
/// </summary>
public class MediaMetadataJson
{
    /// <summary>
    /// Die Metadaten der Medien-Datei als JSON-String.
    /// </summary>
    public string Json { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private MediaMetadataJson(string json) => Json = json;

    public static Result<MediaMetadataJson> Create(string? file)
    {
        var fileInfoContainerResult = MediaFileInfoContainer.Create(file);
        if (fileInfoContainerResult.IsFailure)
        {
            return Result.Failure<MediaMetadataJson>($"Error on reading file: {file}");
        }

        return fileInfoContainerResult.Value.MediaType switch
        {
            MediaType.QuickTimeMov => QuickTimeMetadataContainer.Create(fileInfoContainerResult.Value)
                    .Bind(quickTimeMetadataContainer => FormattedUnicodeJson.Create(quickTimeMetadataContainer.RawMetadata))
                    .Map(formattedUnicodeJson => new MediaMetadataJson(formattedUnicodeJson.Value))
                    .MapError(error => $"Error on trying to get QuickTime metadata: {error}"),

            MediaType.Mpeg4 => Result.Failure<MediaMetadataJson>("MP4 metadata reading not yet implemented."),

            _ => Result.Failure<MediaMetadataJson>("Unsupported file type."),
        };
    }
}
