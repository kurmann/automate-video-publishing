namespace AutomateVideoPublishing.Entities.Metadata.File;

public class FilePair
{
    public FileInfo File1 { get; private set; }
    public FileInfo File2 { get; private set; }

    private FilePair(FileInfo file1, FileInfo file2)
    {
        File1 = file1;
        File2 = file2;
    }

    public static Result<FilePair> Create(string filePath1, string filePath2)
    {
        // Überprüfen, ob die erste Datei existiert.
        var fileInfo1 = new FileInfo(filePath1);
        if (!fileInfo1.Exists)
        {
            return Result.Failure<FilePair>($"File {filePath1} does not exist.");
        }

        // Überprüfen, ob die zweite Datei existiert.
        var fileInfo2 = new FileInfo(filePath2);
        if (!fileInfo2.Exists)
        {
            return Result.Failure<FilePair>($"File {filePath2} does not exist.");
        }

        // Erstellen Sie das Paar und geben Sie es zurück.
        return Result.Success(new FilePair(fileInfo1, fileInfo2));
    }
}