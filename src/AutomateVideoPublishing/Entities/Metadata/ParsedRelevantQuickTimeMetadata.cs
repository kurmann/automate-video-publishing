using System.Globalization;

namespace AutomateVideoPublishing.Parsers;

public class ParsedRelevantQuickTimeMetadata
{
    public Maybe<TimeSpan> Duration { get; }

    private ParsedRelevantQuickTimeMetadata(RelevantQuickTimeMetadata metadata)
    {
        Duration = GetDuration(metadata.DurationString);
    }

    public static Result<ParsedRelevantQuickTimeMetadata> Create(RelevantQuickTimeMetadata metadata)
    {
        if (metadata == null)
        {
            return Result.Failure<ParsedRelevantQuickTimeMetadata>("Die EssentialQuickTimeMetadata dürfen nicht null sein.");
        }

        return Result.Success(new ParsedRelevantQuickTimeMetadata(metadata));
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
