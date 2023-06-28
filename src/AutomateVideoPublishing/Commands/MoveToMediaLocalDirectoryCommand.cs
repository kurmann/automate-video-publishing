using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class MoveToMediaLocalDirectoryCommand : ICommand<FileInfo>
{
    private readonly MetadataTransferCommand _metadataTransferCommand;
    private readonly Subject<FileInfo> _broadcaster = new();

    public static readonly string SeasonPrefix = "Staffel ";
    public static readonly string DefaultAlbum = "TV Shows";

    public IObservable<FileInfo> WhenDataAvailable => _broadcaster.AsObservable();

    public MoveToMediaLocalDirectoryCommand(MetadataTransferCommand metadataTransferCommand) => _metadataTransferCommand = metadataTransferCommand;

    public void Execute(WorkflowContext context)
    {
        _metadataTransferCommand.WhenDataAvailable.Subscribe(
            onNext: transferResult => { /* Nichts zu tun hier, wir warten auf das Abschlussereignis */ },
            onError: ex => _broadcaster.OnError(ex),
            onCompleted: () =>
            {
                // Erzeugen eines gültigen lokalen Medienverzeichnisses basierend auf dem Kontext
                var publishedMediaLocalDirectoryResult = ValidMediaLocalDirectory.Create(context.PublishedMediaLocalDirectory.FullPath);
                if (publishedMediaLocalDirectoryResult.IsFailure)
                {
                    _broadcaster.OnError(new Exception(publishedMediaLocalDirectoryResult.Error));
                }
                else
                {
                    // Durchführung der Dateiverschiebeoperation nur nachdem alle Metadatenübertragungen abgeschlossen sind.
                    MoveFiles(publishedMediaLocalDirectoryResult.Value, context);
                    _broadcaster.OnCompleted();
                }
            });

        _metadataTransferCommand.Execute(context);
    }

    // Verschiebt Dateien entsprechend dem Workflow-Kontext
    private void MoveFiles(ValidMediaLocalDirectory publishedMediaLocalDirectory, WorkflowContext context)
    {
        foreach (var mpeg4File in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var mpeg4MetadataResult = Mpeg4MetadataContainer.Create(mpeg4File.FullName);
            if (mpeg4MetadataResult.IsFailure)
            {
                _broadcaster.OnError(new Exception(mpeg4MetadataResult.Error));
                return;
            }

            var mpeg4Metadata = mpeg4MetadataResult.Value;

            // Hier wird das Album ausgelesen oder Standard-Album
            var album = mpeg4Metadata.Album.HasValue ? mpeg4Metadata.Album.Value : DefaultAlbum;

            // Sicherstellten, dass das Albumverzeichnis existiert
            var albumDirectoryPath = Path.Combine(publishedMediaLocalDirectory.FullPath, album);
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
            var newFilePath = Path.Combine(nextSeasonDetails.NextSeasonDirectory.FullName, mpeg4File.Name);
            try
            {
                mpeg4File.MoveTo(newFilePath);
                _broadcaster.OnNext(new FileInfo(newFilePath));
            }
            catch (Exception ex)
            {
                _broadcaster.OnError(ex);
            }
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
