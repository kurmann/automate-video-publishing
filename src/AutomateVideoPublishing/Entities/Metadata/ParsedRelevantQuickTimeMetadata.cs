using System.Globalization;

namespace AutomateVideoPublishing.Entities.Metadata;

public class ParsedRelevantQuickTimeMetadata : IParsedRelevantMetadata
{
    public SupportedMediaType Type => SupportedMediaType.QuickTime;
    public Maybe<TimeSpan> Duration { get; }
    public Maybe<DateTime> EncodedDate { get; }

    private ParsedRelevantQuickTimeMetadata(RelevantQuickTimeAttributes attributes)
    {
        Duration = TryParseDuration(attributes.Duration);
        EncodedDate = TryParseDateTime(attributes.EncodedDate);
    }

    public static Result<ParsedRelevantQuickTimeMetadata> Create(RelevantQuickTimeMetadata metadata)
    {
        if (metadata == null)
        {
            return Result.Failure<ParsedRelevantQuickTimeMetadata>("Die EssentialQuickTimeMetadata dürfen nicht null sein.");
        }

        return Result.Success(new ParsedRelevantQuickTimeMetadata(metadata.Attributes));
    }

    private static Maybe<TimeSpan> TryParseDuration(string? durationString)
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

    private static Maybe<DateTime> TryParseDateTime(string? dateString)
    {
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
        {
            return result;
        }

        return Maybe<DateTime>.None;
    }
}
