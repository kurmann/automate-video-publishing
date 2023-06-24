namespace AutomateVideoPublishing.Entities;

public class NextSeasonDetails
{
    public PositiveInteger NextSeasonNumber { get; }
    public PositiveInteger NextEpisodeNumber { get; }
    public DirectoryInfo NextSeasonDirectory { get; }

    private NextSeasonDetails(PositiveInteger nextSeasonNumber, PositiveInteger nextEpisodeNumber, DirectoryInfo nextSeasonDirectory)
    {
        NextSeasonNumber = nextSeasonNumber;
        NextEpisodeNumber = nextEpisodeNumber;
        NextSeasonDirectory = nextSeasonDirectory;
    }

    public static NextSeasonDetails Create(ValidTvShowBasedLocalDirectory tvShowBasedLocalDirectory)
    {
        var nextSeasonNumber = tvShowBasedLocalDirectory.CurrentSeasonNumberOrDefault;
        var nextEpisodeNumber = tvShowBasedLocalDirectory.CurrentEpisodeNumberOrDefault;

        // When the current season is already filled up with the max number of episodes, increment the season number and reset episode number
        if (nextEpisodeNumber.Value > 24)
        {
            nextSeasonNumber = PositiveInteger.Create(nextSeasonNumber.Value + 1).Value;
            nextEpisodeNumber = PositiveInteger.Create(1).Value;
        }

        var nextSeasonDirectory = new DirectoryInfo(Path.Combine(tvShowBasedLocalDirectory.Directory.FullName, $"Season {nextSeasonNumber.Value}"));

        if (!nextSeasonDirectory.Exists)
        {
            nextSeasonDirectory.Create();
        }

        return new NextSeasonDetails(nextSeasonNumber, nextEpisodeNumber, nextSeasonDirectory);
    }
}
