using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class MoveToMediaLocalDirectoryCommand : ICommand<FileInfo>
{
    private readonly UpdateMetadataCommand _command;
    private readonly Subject<FileInfo> _broadcaster = new();

    public static readonly string SeasonPrefix = "Staffel ";
    public static readonly string DefaultAlbum = "TV Shows";

    public IObservable<FileInfo> WhenDataAvailable => _broadcaster.AsObservable();

    public MoveToMediaLocalDirectoryCommand(UpdateMetadataCommand command) => this._command = command;

    public void Execute(WorkflowContext context)
    {
        _command.WhenDataAvailable
            .Subscribe(
                onNext: transferResult => MoveFile(context, transferResult.File, transferResult.UpdatedMetadata),
                onError: ex => _broadcaster.OnError(ex),
                onCompleted: () => _broadcaster.OnCompleted());

        _command.Execute(context);
    }

    
    /// <summary>
    /// Verschiebt die Datei ins Zielverzeichnis des Workflow-Context und in die Unterverzeichnisse abhängig
    /// vom Albumnamen (Metadata) und der aktuellen Staffelnummer im Zielverzeichnis.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="file"></param>
    /// <param name="updatedMetadata"></param>
    private void MoveFile(WorkflowContext context, FileInfo file, Mpeg4MetadataCollection updatedMetadata)
    {
        // Hier wird das Album ausgelesen oder Standard-Album
        var album = updatedMetadata.Album.HasValue ? updatedMetadata.Album.Value : DefaultAlbum;

        // Sicherstellten, dass das Albumverzeichnis existiert
        var albumDirectoryPath = Path.Combine(context.PublishedMediaLocalDirectory.FullPath, album);
        var albumDirectory = CreateDirectoryIfNotExists(albumDirectoryPath);

        // Erstellen eines TV Show basierten Verzeichnisses
        var tvShowDirectoryResult = ValidTvShowBasedLocalDirectory.Create(albumDirectoryPath);
        if (tvShowDirectoryResult.IsFailure)
        {
            _broadcaster.OnError(new Exception(tvShowDirectoryResult.Error));
            return;
        }

        // Berechnen der Details für die nächste Staffel
        var nextSeasonDetails = NextSeasonDetails.Create(tvShowDirectoryResult.Value);

        // Verschieben der Datei
        var newFilePath = Path.Combine(nextSeasonDetails.NextSeasonDirectory.FullName, file.Name);
        try
        {
            file.MoveTo(newFilePath);
            _broadcaster.OnNext(new FileInfo(newFilePath));
        }
        catch (Exception ex)
        {
            _broadcaster.OnError(ex);
        }
    }

    // Erstellt ein Verzeichnis, wenn es nicht existiert
    private DirectoryInfo CreateDirectoryIfNotExists(string path)
    {
        var directory = new DirectoryInfo(path);
        if (!directory.Exists)
        {
            directory.Create();
        }
        return directory;
    }
}
