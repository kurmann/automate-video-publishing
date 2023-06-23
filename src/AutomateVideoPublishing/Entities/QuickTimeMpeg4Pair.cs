namespace AutomateVideoPublishing.Entities
{
    public class QuickTimeMpeg4Pair
    {
        public QuickTimeMetadataContainer Source { get; private set; }
        public Mpeg4MetadataContainer Target { get; private set; }

        private QuickTimeMpeg4Pair(QuickTimeMetadataContainer source, Mpeg4MetadataContainer target)
        {
            Source = source;
            Target = target;
        }

        public static Result<QuickTimeMpeg4Pair> Create(string sourceFilePath, string? targetDirectoryPath)
        {
            var sourceResult = QuickTimeMetadataContainer.Create(sourceFilePath);
            if (sourceResult.IsFailure)
            {
                return Result.Failure<QuickTimeMpeg4Pair>(sourceResult.Error);
            }

            var targetDirectoryResult = ValidMpeg4Directory.Create(targetDirectoryPath);
            if (targetDirectoryResult.IsFailure)
            {
                return Result.Failure<QuickTimeMpeg4Pair>(targetDirectoryResult.Error);
            }

            var targetFileName = sourceResult.Value.FileInfo.Name.ToLower().Replace(".mov", "");

            var mpeg4Files = new[] { ".m4v", ".mp4" };

            var matchingFiles = targetDirectoryResult.Value.Mpeg4Files
                .Where(f => string.Equals(f.Name.ToLower(), targetFileName + mpeg4Files[0], StringComparison.OrdinalIgnoreCase)
                        || string.Equals(f.Name.ToLower(), targetFileName + mpeg4Files[1], StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (matchingFiles.Count > 1)
            {
                return Result.Failure<QuickTimeMpeg4Pair>($"Mehrere passende Mpeg4-Dateien für {sourceResult.Value.FileInfo.Name} gefunden.");
            }
            else if (matchingFiles.Count == 0)
            {
                return Result.Failure<QuickTimeMpeg4Pair>($"Keine passende Mpeg4-Datei für {sourceResult.Value.FileInfo.Name} gefunden.");
            }

            var targetFileResult = Mpeg4MetadataContainer.Create(matchingFiles[0].FullName);
            if (targetFileResult.IsFailure)
            {
                return Result.Failure<QuickTimeMpeg4Pair>(targetFileResult.Error);
            }

            return Result.Success(new QuickTimeMpeg4Pair(sourceResult.Value, targetFileResult.Value));
        }
    }
}
