using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Ein Befehl, der eine Sequenz von Metadatenpaaren aus einer QuickTime-Datei und den zugehörigen MPEG4-Dateien generiert.
/// </summary>
public class PairCollectorCommand : ICommand<QuickTimeMpeg4MetadataContainerPair>
{
    private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
    private readonly Subject<QuickTimeMpeg4MetadataContainerPair> _broadcaster = new();

    /// <summary>
    /// Ein Observable, das ein neues QuickTimeMpeg4MetadataContainerPair ausgibt, sobald es verfügbar ist.
    /// </summary>
    public IObservable<QuickTimeMpeg4MetadataContainerPair> WhenDataAvailable => _broadcaster.AsObservable();

    /// <summary>
    /// Erzeugt eine neue Instanz der PairCollectorCommand-Klasse.
    /// </summary>
    /// <param name="quickTimeCommand">Ein Befehl, der QuickTime-Metadaten liest.</param>
    public PairCollectorCommand(QuickTimeMetadataReadCommand quickTimeCommand) => _quickTimeCommand = quickTimeCommand;

    /// <summary>
    /// Führt den Befehl aus, der eine Sequenz von QuickTimeMpeg4MetadataContainerPair ausgibt.
    /// </summary>
    /// <param name="context">Der Workflow-Kontext.</param>
    public void Execute(WorkflowContext context)
    {
        // Bei Verfügbarkeit der QuickTime-Metadaten...
        _quickTimeCommand.WhenDataAvailable
            // ...wird für jeden QuickTime-Metadatencontainer eine Sequenz von Metadatenpaaren erstellt.
            .SelectMany(quicktimeContainer =>
            {
                var targetDirectoryPath = context.PublishedMpeg4Directory.Directory.FullName;

                // Erstellt eine Gruppe von Metadaten, die eine QuickTime-Datei und die zugehörigen MPEG4-Dateien repräsentiert.
                var groupResult = QuickTimeToMpeg4VersionsMetadataGroup.Create(quicktimeContainer.FileInfo.FullName, targetDirectoryPath);

                // Im Fehlerfall wird ein Fehlerereignis ausgelöst.
                if (groupResult.IsFailure)
                {
                    _broadcaster.OnError(new Exception(groupResult.Error));
                    return new List<QuickTimeMpeg4MetadataContainerPair>().ToObservable();
                }

                var pairs = new List<QuickTimeMpeg4MetadataContainerPair>();

                // Für jede MPEG4-Datei in der Gruppe wird ein Metadatenpaar erstellt.
                foreach (var mpeg4MetadataContainer in groupResult.Value.Mpeg4MetadataContainers)
                {
                    var pairResult = QuickTimeMpeg4MetadataContainerPair.Create(quicktimeContainer.FileInfo.FullName, mpeg4MetadataContainer.FileInfo.FullName);

                    // Im Fehlerfall wird ein Fehlerereignis ausgelöst.
                    if (pairResult.IsFailure)
                    {
                        _broadcaster.OnError(new Exception(pairResult.Error));
                    }
                    else
                    {
                        pairs.Add(pairResult.Value);
                    }
                }

                // Konvertiert die Liste von Metadatenpaaren in ein Observable.
                return pairs.ToObservable();
            })
            // Jedes erstellte Metadatenpaar wird ausgegeben.
            .Subscribe(_broadcaster.OnNext);
        
        // Führt den QuickTimeMetadataReadCommand aus, um die Generierung von Metadatenpaaren zu starten.
        _quickTimeCommand.Execute(context);
    }
}
