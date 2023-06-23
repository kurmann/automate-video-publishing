namespace AutomateVideoPublishing.Entities;

public class QuickTimeMpeg4MetadataPair
{
    private readonly QuickTimeMpeg4Pair _pair;

    public QuickTimeMetadataContainer QuickTimeMetadataContainer => _pair.Source;
    public Mpeg4MetadataContainer Mpeg4MetadataContainer => _pair.Target;

    public bool IsYearSame
    {
        get
        {
            var sourceYear = _pair.Source.YearByFilename;
            var targetYear = _pair.Target.Year;

            if (sourceYear.HasValue && targetYear.HasValue)
            {
                return sourceYear.Value == targetYear.Value;
            }

            return false;
        }
    }

    public bool IsDescriptionSame
    {
        get
        {
            var sourceDescription = _pair.Source.Description;
            var targetDescription = _pair.Target.Description;

            if (sourceDescription.HasValue && targetDescription.HasValue)
            {
                return sourceDescription.Value == targetDescription;
            }

            return false;
        }
    }

    private QuickTimeMpeg4MetadataPair(QuickTimeMpeg4Pair pair) => _pair = pair;

    public static Result<QuickTimeMpeg4MetadataPair> Create(string sourceFilePath, string targetDirectoryPath)
    {
        var pairResult = QuickTimeMpeg4Pair.Create(sourceFilePath, targetDirectoryPath);

        if (pairResult.IsFailure)
        {
            return Result.Failure<QuickTimeMpeg4MetadataPair>(pairResult.Error);
        }

        return Result.Success(new QuickTimeMpeg4MetadataPair(pairResult.Value));
    }
}
