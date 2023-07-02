using System.Text.RegularExpressions;

namespace AutomateVideoPublishing.Entities;

public class UpdateMetadataBaseData : ValueObject
{
    private static readonly Regex DateRegex = new Regex(@"(\d{4})(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])");

    public Maybe<string> Description { get; }
    public Maybe<DateTime> Date { get; }

    private UpdateMetadataBaseData(Maybe<string> description, Maybe<DateTime> date)
    {
        Description = description;
        Date = date;
    }

    public static Result<UpdateMetadataBaseData> Create(string description, string filename)
    {
        var maybeDescription = string.IsNullOrWhiteSpace(description) ? Maybe<string>.None : Maybe<string>.From(description);
        var maybeDate = ParseDateFromFilename(filename);

        return Result.Success(new UpdateMetadataBaseData(maybeDescription, maybeDate));
    }

    private static Maybe<DateTime> ParseDateFromFilename(string filename)
    {
        var match = DateRegex.Match(Path.GetFileNameWithoutExtension(filename));
        if (!match.Success) return Maybe<DateTime>.None;

        int year = int.Parse(match.Groups[1].Value);
        int month = int.Parse(match.Groups[2].Value);
        int day = int.Parse(match.Groups[3].Value);

        return Maybe<DateTime>.From(new DateTime(year, month, day));
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Description.HasValue ? Description.Value : string.Empty;
        yield return Date.HasValue ? Date.Value : default(DateTime);
    }
}
