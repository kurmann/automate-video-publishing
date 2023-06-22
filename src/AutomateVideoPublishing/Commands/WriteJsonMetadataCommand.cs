using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace AutomateVideoPublishing.Commands;

public class WriteJsonMetadataCommand : ICommand<string>
{
    private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
    private readonly Mpeg4DirectoryMetadataReadCommand _mpeg4Command;

    /// <summary>
    /// Subject, das den Dateipfad von erfolgreich erstellten JSON-Dateien sendet.
    /// </summary>
    private readonly Subject<string> _broadcaster = new Subject<string>();

    /// <summary>
    /// Beobachtbare Strömung von Dateipfaden aus erfolgreich erstellten JSON-Dateien.
    /// </summary>
    public IObservable<string> WhenDataAvailable => _broadcaster.AsObservable();

    public WriteJsonMetadataCommand(QuickTimeMetadataReadCommand quickTimeCommand, Mpeg4DirectoryMetadataReadCommand mpeg4Command)
    {
        _quickTimeCommand = quickTimeCommand;
        _mpeg4Command = mpeg4Command;
    }

    public void Execute(WorkflowContext context)
    {
        // Zusammenführen der Datenströme beider Befehle zu einem
        _quickTimeCommand.WhenDataAvailable
            .SelectMany(container => WriteJsonAsync(container.FileInfo).ToObservable())
            .Merge(_mpeg4Command.WhenDataAvailable.SelectMany(container => WriteJsonAsync(container.FileInfo).ToObservable()))
            .Subscribe(_broadcaster);

        _quickTimeCommand.Execute(context);
        _mpeg4Command.Execute(context);

        _broadcaster.OnCompleted();
    }

    /// <summary>
    /// Schreibt JSON-Daten in eine Datei und gibt den Dateipfad zurück. Tritt ein Fehler auf, wird ein Fehler gesendet und ein leerer String zurückgegeben.
    /// </summary>
    /// <param name="fileInfo">Die Dateiinformationen, aus denen die JSON-Daten generiert werden.</param>
    /// <returns>Den Dateipfad der erfolgreich erstellten JSON-Datei oder einen leeren String im Fehlerfall.</returns>
    private async Task<string> WriteJsonAsync(FileInfo fileInfo)
    {
        var jsonResult = MediaMetadataJson.Create(fileInfo.FullName);
        if (jsonResult.IsSuccess)
        {
            var jsonFile = new FileInfo(Path.ChangeExtension(fileInfo.FullName, ".json"));
            await File.WriteAllTextAsync(jsonFile.FullName, jsonResult.Value.Json);
            return jsonFile.FullName;
        }
        else
        {
            _broadcaster.OnError(new Exception($"Json-Datei aus Datei {fileInfo} konnte nicht generiert werden."));
            return string.Empty;
        }
    }
}
