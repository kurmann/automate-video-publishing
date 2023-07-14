using System.Globalization;

namespace AutomateVideoPublishing.Parsers;

public class ParsedQuickTimeMetadata
{
    public Maybe<TimeSpan> Duration { get; }

    private ParsedQuickTimeMetadata(EssentialQuickTimeMetadata metadata)
    {
        Duration = GetDuration(metadata.DurationString);
    }

    public static Result<ParsedQuickTimeMetadata> Create(EssentialQuickTimeMetadata metadata)
    {
        if (metadata == null)
        {
            return Result.Failure<ParsedQuickTimeMetadata>("Die EssentialQuickTimeMetadata dürfen nicht null sein.");
        }

        return Result.Success(new ParsedQuickTimeMetadata(metadata));
    }

    private static Maybe<TimeSpan> GetDuration(string? durationString)
    {
        if (string.IsNullOrWhiteSpace(durationString))
        {
            return Maybe<TimeSpan>.None;
        }

        // Konvertiert die Dauer-String in ein TimeSpan-Objekt, wenn es im richtigen Format ist.
        if (TimeSpan.TryParseExact(durationString, @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture, out var duration))
        {
            return Maybe<TimeSpan>.From(duration);
        }

        return Maybe<TimeSpan>.None;
    }
}
