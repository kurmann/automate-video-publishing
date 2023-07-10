namespace AutomateVideoPublishing.Entities.Metadata;

public class ParsedQuicktimeMetadata
{
    // Properties
    public Maybe<string> Title { get; }
    public Maybe<string> Description { get; }
    public IReadOnlyDictionary<string, List<KeyValuePair<string, string?>>> RawMetadata { get; }

    // Private Constructor
    private ParsedQuicktimeMetadata(Maybe<string> title, Maybe<string> description, Dictionary<string, List<KeyValuePair<string, string?>>> sections)
    {
        Title = title;
        Description = description;
        RawMetadata = sections;
    }

    public static Result<ParsedQuicktimeMetadata> Create(MediaInfoMetadataLineOutput mediaInfoMetadataLineOutput)
    {
        if (mediaInfoMetadataLineOutput == null)
            return Result.Failure<ParsedQuicktimeMetadata>("mediaInfoMetadataLineOutput cannot be null.");

        var sections = mediaInfoMetadataLineOutput.Sections;

        var title = Maybe<string>.None;
        var description = Maybe<string>.None;

        if (sections.ContainsKey("General"))
        {
            foreach (var item in sections["General"])
            {
                if (item.Key == "Title" && item.Value != null)
                    title = Maybe<string>.From(item.Value);
                if (item.Key == "Description" && item.Value != null)
                    description = Maybe<string>.From(item.Value);
            }
        }

        return new ParsedQuicktimeMetadata(title, description, sections);
    }
}
