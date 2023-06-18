/// <summary>
/// Repräsentiert ein gültiges MPEG-4-Verzeichnis.
/// Diese Klasse stellt sicher, dass das angegebene Verzeichnis existiert und enthält eine Sammlung von MPEG-4-Dateien innerhalb dieses Verzeichnisses.
/// </summary>
public class ValidMpeg4Directory
{
    public DirectoryInfo Directory { get; }
    public IReadOnlyList<FileInfo> Mpeg4Files { get; }

    private ValidMpeg4Directory(DirectoryInfo directory, List<FileInfo> mpeg4Files)
    {
        Directory = directory;
        Mpeg4Files = mpeg4Files.AsReadOnly();
    }

    public static Result<ValidMpeg4Directory> Create(string? directoryPath)
    {
        directoryPath = string.IsNullOrWhiteSpace(directoryPath) ? "." : directoryPath;

        var directory = new DirectoryInfo(directoryPath);
        if (!directory.Exists)
        {
            return Result.Failure<ValidMpeg4Directory>("MPEG-4 directory does not exist.");
        }

        var mpeg4Files = directory.EnumerateFiles("*.mp4").Concat(directory.EnumerateFiles("*.m4v")).ToList();

        return Result.Success(new ValidMpeg4Directory(directory, mpeg4Files));
    }
}
