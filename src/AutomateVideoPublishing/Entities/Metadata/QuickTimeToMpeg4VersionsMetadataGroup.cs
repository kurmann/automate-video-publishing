namespace AutomateVideoPublishing.Entities.Metadata;

public class QuickTimeToMpeg4VersionsMetadataGroup
{
    public QuickTimeMetadataContainer QuickTimeMetadataContainer { get; }
    public List<Mpeg4MetadataContainer> Mpeg4MetadataContainers { get; }

    private QuickTimeToMpeg4VersionsMetadataGroup(QuickTimeMetadataContainer quickTimeMetadataContainer, List<Mpeg4MetadataContainer> mpeg4MetadataContainers)
    {
        QuickTimeMetadataContainer = quickTimeMetadataContainer;
        Mpeg4MetadataContainers = mpeg4MetadataContainers;
    }

    public static Result<QuickTimeToMpeg4VersionsMetadataGroup> Create(string quickTimeFilePath, string targetDirectoryPath)
    {
        var quickTimeContainerResult = QuickTimeMetadataContainer.Create(quickTimeFilePath);

        if (quickTimeContainerResult.IsFailure)
        {
            return Result.Failure<QuickTimeToMpeg4VersionsMetadataGroup>($"Error on reading QuickTime metadata: {quickTimeContainerResult.Error}");
        }

        var validMpeg4DirectoryResult = ValidMpeg4Directory.Create(targetDirectoryPath);

        if (validMpeg4DirectoryResult.IsFailure)
        {
            return Result.Failure<QuickTimeToMpeg4VersionsMetadataGroup>($"Error on reading MPEG-4 metadata: {validMpeg4DirectoryResult.Error}");
        }

        var quickTimeFileInfo = new FileInfo(quickTimeFilePath);

        // Extract the date and name from the source file.
        var dateAndName = Path.GetFileNameWithoutExtension(quickTimeFileInfo.Name);
        var mpeg4MetadataContainers = new List<Mpeg4MetadataContainer>();

        // Look for all mpeg4 files in the target directory that start with the same date and name.
        var mpeg4Files = validMpeg4DirectoryResult.Value.Mpeg4Files
                            .Where(f => f.Name.StartsWith($"{dateAndName}", StringComparison.InvariantCultureIgnoreCase));

        foreach (var mpeg4File in mpeg4Files)
        {
            var mpeg4ContainerResult = Mpeg4MetadataContainer.Create(mpeg4File.FullName);

            if (mpeg4ContainerResult.IsFailure)
            {
                return Result.Failure<QuickTimeToMpeg4VersionsMetadataGroup>(mpeg4ContainerResult.Error);
            }

            mpeg4MetadataContainers.Add(mpeg4ContainerResult.Value);
        }

        if (!mpeg4MetadataContainers.Any())
        {
            return Result.Failure<QuickTimeToMpeg4VersionsMetadataGroup>("No matching mpeg4 files found in target directory.");
        }

        return Result.Success(new QuickTimeToMpeg4VersionsMetadataGroup(quickTimeContainerResult.Value, mpeg4MetadataContainers));
    }
}
