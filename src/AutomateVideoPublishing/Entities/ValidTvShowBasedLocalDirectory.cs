namespace AutomateVideoPublishing.Entities;

public class ValidTvShowBasedLocalDirectory
{
    public DirectoryInfo Directory { get; }
    public PositiveInteger CurrentSeasonNumberOrDefault { get; }
    public PositiveInteger CurrentEpisodeNumberOrDefault { get; }

    private ValidTvShowBasedLocalDirectory(DirectoryInfo directoryInfo, PositiveInteger currentSeasonNumber, PositiveInteger currentEpisodeNumber)
    {
        Directory = directoryInfo;
        CurrentSeasonNumberOrDefault = currentSeasonNumber;
        CurrentEpisodeNumberOrDefault = currentEpisodeNumber;
    }

    public static Result<ValidTvShowBasedLocalDirectory> Create(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure<ValidTvShowBasedLocalDirectory>("Directory path of published local media cannot be null or empty.");
        }

        var directoryInfo = new DirectoryInfo(path);

        if (!directoryInfo.Exists)
        {
            return Result.Failure<ValidTvShowBasedLocalDirectory>($"Directory does not exist: {directoryInfo.FullName}");
        }

        var currentSeasonNumber = GetCurrentSeasonNumber(directoryInfo);
        var currentEpisodeNumber = GetCurrentEpisodeNumber(directoryInfo, currentSeasonNumber);

        return Result.Success(new ValidTvShowBasedLocalDirectory(directoryInfo, currentSeasonNumber, currentEpisodeNumber));
    }

    private static PositiveInteger GetCurrentSeasonNumber(DirectoryInfo directoryInfo)
    {
        var seasonDirectories = directoryInfo.GetDirectories("Season *");

        var currentSeasonNumber = seasonDirectories.Select(d =>
        {
            if (int.TryParse(d.Name.Split(' ')[1], out int number))
                return PositiveInteger.Create(number).Value;
            return PositiveInteger.Create(1).Value;
        }).DefaultIfEmpty(PositiveInteger.Create(1).Value).Max();

        return currentSeasonNumber ?? PositiveInteger.Default;
    }

    private static PositiveInteger GetCurrentEpisodeNumber(DirectoryInfo directoryInfo, PositiveInteger currentSeasonNumber)
    {
        var seasonDirectoryPath = Path.Combine(directoryInfo.FullName, $"Season {currentSeasonNumber}");
        var seasonDirectory = new DirectoryInfo(seasonDirectoryPath);

        // Check if directory exists before getting files.
        if (!seasonDirectory.Exists)
        {
            return PositiveInteger.Create(1).Value; // return default value
        }

        if (seasonDirectory.GetFiles().Length > 24)
        {
            return PositiveInteger.Create(1).Value;
        }

        return PositiveInteger.Create(seasonDirectory.GetFiles().Length + 1).Value;
    }

}
