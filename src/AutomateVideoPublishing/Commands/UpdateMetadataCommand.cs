using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Entities.AtomicParsley;
using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Ein Befehl zum Aktualisieren der Metadaten von Mpeg4-Dateien.
/// </summary>
public class UpdateMetadataCommand : ICommand<UpdateMetadataResult>
{
    private readonly Subject<UpdateMetadataResult> _broadcaster = new(); // Subject für die Info der bearbeiteten Dateien
    private readonly CollectMetadataToUpdateCommand _collectMetadataToUpdateCommand;
    private readonly ProcessManager _processManager;

    /// <summary>
    /// Liefert Daten zu bearbeiteten Dateien, sobald sie verfügbar sind.
    /// </summary>
    public IObservable<UpdateMetadataResult> WhenDataAvailable => _broadcaster.AsObservable();

    /// <summary>
    /// Erstellt eine neue Instanz des UpdateMetadataCommand.
    /// </summary>
    /// <param name="collectMetadataToUpdateCommand">Ein Befehl zum Sammeln von Metadaten zur Aktualisierung.</param>
    public UpdateMetadataCommand(CollectMetadataToUpdateCommand collectMetadataToUpdateCommand)
    {
        _collectMetadataToUpdateCommand = collectMetadataToUpdateCommand;
        _processManager = new ProcessManager();
    }

    /// <summary>
    /// Führt den Befehl aus.
    /// </summary>
    /// <param name="context">Der Kontext des Workflows.</param>
    public void Execute(WorkflowContext context)
    {
        _collectMetadataToUpdateCommand.WhenDataAvailable.Subscribe(onNext: metadataBaseData =>
        {
            if (metadataBaseData.Date.HasValue)
            {
                string fileName = metadataBaseData.FileInfo.FullName;
                var dayArguments = AtomicParsleyUpdateMetadataArguments.CreateOverwriteDay(fileName, metadataBaseData.Date.Value);

                _processManager.StartNewProcess("AtomicParsley", dayArguments.Arguments);
            }

            if (metadataBaseData.Description.HasValue)
            {
                string fileName = metadataBaseData.FileInfo.FullName;
                var descriptionArguments = AtomicParsleyUpdateMetadataArguments.CreateOverwriteDescription(fileName, metadataBaseData.Description.Value);

                _processManager.StartNewProcess("AtomicParsley", descriptionArguments.Arguments);
            }

            var result = UpdateMetadataResult.Create(metadataBaseData.FileInfo.FullName, metadataBaseData.Date, metadataBaseData.Description);
            _broadcaster.OnNext(result);
        });

        _collectMetadataToUpdateCommand.Execute(context);
    }
}

/// <summary>
/// Ein Ergebnis der Aktualisierung der Metadaten.
/// </summary>
public class UpdateMetadataResult
{
    public string FileName { get; }
    public Maybe<DateTime> Date { get; }
    public Maybe<string> Description { get; }

    /// <summary>
    /// Eine Zusammenfassung der Aktualisierung der Metadaten.
    /// </summary>
    public string SummaryMessage
    {
        get
        {
            var datePart = Date.HasValue ? $"Date updated to {Date.Value:yyyy-MM-dd HH:mm:ss}" : "Date not updated";
            var descPart = Description.HasValue ? $", Description updated to {Description.Value}" : ", Description not updated";
            return $"In the file {FileName}, " + datePart + descPart;
        }
    }

    private UpdateMetadataResult(string fileName, Maybe<DateTime> date, Maybe<string> description)
    {
        FileName = fileName;
        Date = date;
        Description = description;
    }

    /// <summary>
    /// Erstellt eine neue Instanz des UpdateMetadataResult.
    /// </summary>
    /// <param name="fileName">Der Name der Datei, deren Metadaten aktualisiert wurden.</param>
    /// <param name="date">Das aktualisierte Datum.</param>
    /// <param name="description">Die aktualisierte Beschreibung.</param>
    public static UpdateMetadataResult Create(string fileName, Maybe<DateTime> date, Maybe<string> description)
        => new UpdateMetadataResult(fileName, date, description);
}
