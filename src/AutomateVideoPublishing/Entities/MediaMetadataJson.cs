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
    public string Value { get; private set; }

    // Privater Konstruktor, der nur innerhalb dieser Klasse aufgerufen werden kann.
    private MediaMetadataJson(string json)
    {
        Value = json;
    }

    public static Result<MediaMetadataJson> Create(string? file)
    {
        var fileInfoContainerResult = MediaFileInfoContainer.Create(file);

        if (fileInfoContainerResult.IsFailure)
        {
            return Result.Failure<MediaMetadataJson>($"Error on reading file: {file}");
        }

        // quicktime and mp4 have different tools to read metadata from
        switch (fileInfoContainerResult.Value.MediaType)
        {
            case MediaType.QuickTimeMov:
                return QuickTimeMetadataContainer.Create(fileInfoContainerResult.Value)
                    .Bind(quickTimeMetadataContainer => FormattedUnicodeJson.Create(quickTimeMetadataContainer.RawMetadata))
                    .Map(formattedUnicodeJson => new MediaMetadataJson(formattedUnicodeJson.Value))
                    .MapError(error => $"Error on trying to get QuickTime metadata: {error}");
            case MediaType.Mpeg4:
                // Hier sollte die Logik zum Lesen von MP4-Metadaten eingefügt werden. 
                // Zum Zwecke dieses Beispiels wird ein vorübergehender Fehler zurückgegeben.
                return Result.Failure<MediaMetadataJson>("MP4 metadata reading not yet implemented.");
            default:
                return Result.Failure<MediaMetadataJson>("Unsupported file type.");
        }
    }

}
