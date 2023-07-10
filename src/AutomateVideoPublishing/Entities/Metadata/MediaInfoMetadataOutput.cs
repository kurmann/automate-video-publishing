namespace AutomateVideoPublishing.Entities.Metadata
{
    public class MediaInfoMetadataOutput
    {
        public Dictionary<string, string?> GeneralSection { get; }

        public Maybe<string> Description =>
            GeneralSection.TryGetValue("Description", out var description) && !string.IsNullOrEmpty(description)
                ? Maybe.From(description)
                : Maybe<string>.None;

        public Maybe<string> Title =>
            GeneralSection.TryGetValue("Title", out var title) && !string.IsNullOrEmpty(title)
                ? Maybe.From(title)
                : Maybe<string>.None;

        private MediaInfoMetadataOutput(Dictionary<string, string?> generalSection) => GeneralSection = generalSection;

        public static Result<MediaInfoMetadataOutput> Create(IEnumerable<string> lines)
        {
            try
            {
                var generalSection = ExtractGeneralSection(lines);
                return new MediaInfoMetadataOutput(generalSection);
            }
            catch (Exception ex)
            {
                // Loggen oder behandeln Sie den Fehler hier, falls notwendig
                return Result.Failure<MediaInfoMetadataOutput>(ex.Message);
            }
        }

        private static Dictionary<string, string?> ExtractGeneralSection(IEnumerable<string> lines)
        {
            var result = new Dictionary<string, string?>();

            bool isGeneralSection = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("General"))
                {
                    isGeneralSection = true;
                    continue;
                }

                // Check if the line is empty, in which case, we assume that the 'General' section has ended.
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }

                if (isGeneralSection)
                {
                    var parts = line.Split(':', 2);
                    if (parts.Length == 2) // Prüfe ob die Zeile ein gültiges Schlüssel-Wert-Paar ist
                    {
                        var key = parts[0].Trim();
                        var value = parts[1].Trim();

                        if (!string.IsNullOrWhiteSpace(key)) // Überprüfe, ob der Schlüssel nicht leer oder null ist
                        {
                            if (!result.ContainsKey(key))
                            {
                                result[key] = value;
                            }
                            else
                            {
                                result[key] += "; " + value; // Füge zusätzliche Werte mit Semikolon getrennt hinzu
                            }
                        }
                    }
                }
            }

            return result;
        }


    }
}
