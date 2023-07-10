namespace AutomateVideoPublishing.Entities.Metadata
{
    public class MediaInfoMetadataLineOutput
    {
        public Dictionary<string, List<KeyValuePair<string, string?>>> Sections { get; }

        private MediaInfoMetadataLineOutput(Dictionary<string, List<KeyValuePair<string, string?>>> sections) => Sections = sections;

        public static Result<MediaInfoMetadataLineOutput, string> Create(IEnumerable<string> lines)
        {
            var sections = new Dictionary<string, List<KeyValuePair<string, string?>>>();
            var currentSection = string.Empty;
            var currentProperties = new List<KeyValuePair<string, string?>>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) // New section begins
                {
                    if (!string.IsNullOrWhiteSpace(currentSection))
                    {
                        sections[currentSection] = currentProperties;
                        currentProperties = new List<KeyValuePair<string, string?>>();
                    }
                }
                else
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length == 1) // This line is the name of the section
                    {
                        currentSection = parts[0].Trim();
                    }
                    else if (parts.Length == 2) // This line is a property of the current section
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        currentProperties.Add(new KeyValuePair<string, string?>(key, value));
                    }
                }
            }

            // Add the last section
            if (!string.IsNullOrWhiteSpace(currentSection) && currentProperties.Any())
            {
                sections[currentSection] = currentProperties;
            }

            return new MediaInfoMetadataLineOutput(sections);
        }

        public Maybe<string> Description =>
            ExtractSingleValueFromSection("General", "Description");

        public Maybe<string> Title =>
            ExtractSingleValueFromSection("General", "Title");

        private Maybe<string> ExtractSingleValueFromSection(string sectionName, string keyName)
        {
            if (Sections.TryGetValue(sectionName, out var section))
            {
                var keyValuePairs = section.Where(pair => pair.Key == keyName).ToList();
                if (keyValuePairs.Any())
                {
                    return string.Join("; ", keyValuePairs.Select(pair => pair.Value));
                }
            }
            return Maybe<string>.None;
        }
    }
}
