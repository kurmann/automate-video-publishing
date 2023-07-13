using System.Diagnostics;

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
                var trimmedLine = line.Trim();

                // Check if this line is a section header
                if (trimmedLine == "General" || trimmedLine == "Video" || trimmedLine == "Audio" || trimmedLine == "Other" || trimmedLine == "Menu")
                {
                    // Save current section and start a new one
                    if (!string.IsNullOrWhiteSpace(currentSection))
                    {
                        currentProperties = RemoveDuplicateKeyValuePairs(currentProperties);
                        sections[currentSection] = currentProperties;
                        currentProperties = new List<KeyValuePair<string, string?>>();
                    }
                    currentSection = trimmedLine;
                }
                else if (!string.IsNullOrWhiteSpace(trimmedLine)) // This line is a property of the current section
                {
                    var parts = trimmedLine.Split(':', 2);
                    if (parts.Length == 2)
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
                currentProperties = RemoveDuplicateKeyValuePairs(currentProperties);
                sections[currentSection] = currentProperties;
            }

            return new MediaInfoMetadataLineOutput(sections);
        }


        private static List<KeyValuePair<string, string?>> RemoveDuplicateKeyValuePairs(List<KeyValuePair<string, string?>> keyValuePairs)
        {
            var seen = new HashSet<KeyValuePair<string, string?>>();
            var result = new List<KeyValuePair<string, string?>>();

            foreach (var pair in keyValuePairs)
            {
                if (seen.Add(pair))
                {
                    result.Add(pair);
                }
            }

            return result;
        }


        public Maybe<string> Description =>
            ExtractSingleValueFromSection("General", "Description");

        public Maybe<string> Title =>
            ExtractSingleValueFromSection("General", "Title");

        /// <summary>
        /// Versucht, einen bestimmten Wert, identifiziert durch 'keyName', aus einer spezifischen Sektion, 
        /// identifiziert durch 'sectionName', zu extrahieren.
        /// Gibt die Werte aller übereinstimmenden Schlüssel als einen einzigen, durch Semikolons 
        /// getrennten String zurück oder Maybe<string>.None, wenn kein übereinstimmender Schlüssel gefunden wurde.
        /// </summary>
        /// <param name="sectionName">Der Name der Section (bspw. "Video" oder "Audio"</param>
        /// <param name="keyName">Der Schlüsselname bspw. "Title" oder "Duration"</param>
        /// <returns></returns>
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
