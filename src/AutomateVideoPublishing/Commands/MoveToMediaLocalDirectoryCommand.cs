using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class MoveToMediaLocalDirectoryCommand : ICommand<string> // ICommand<T> sollte das erwartete Ergebnis Ihrer Operation repräsentieren. 
                                                                 // Ich verwende string als Platzhalter
{
    private readonly Subject<string> _broadcaster = new();

    public IObservable<string> WhenDataAvailable => _broadcaster.AsObservable();

    public void Execute(WorkflowContext context)
    {
        // Erstellt das PublishedMediaLocalDirectory anhand des Workflow-Kontexts
        var publishedMediaLocalDirectoryResult = ValidMediaLocalDirectory.Create(context.PublishedMediaLocalDirectory.FullPath);
        if (publishedMediaLocalDirectoryResult.IsFailure)
        {
            _broadcaster.OnError(new Exception(publishedMediaLocalDirectoryResult.Error));
            return;
        }

        // Hier wird die eigentliche Dateibewegungsoperation ausgeführt. Dies ist derzeit nicht implementiert
        // und dient nur der Darstellung des grundlegenden Aufbaus des Befehls.
        var fileMovingResult = MoveFiles(publishedMediaLocalDirectoryResult.Value);
        if (fileMovingResult.IsFailure)
        {
            _broadcaster.OnError(new Exception(fileMovingResult.Error));
            return;
        }

        // Sendet das Ergebnis der Operation
        _broadcaster.OnNext(fileMovingResult.Value);

        _broadcaster.OnCompleted();
    }

    // Die MoveFiles-Methode soll die eigentliche Dateibewegung durchführen.
    // Derzeit nicht implementiert und muss entsprechend Ihrer spezifischen Anforderungen ausgefüllt werden.
    private Result<string> MoveFiles(ValidMediaLocalDirectory publishedMediaLocalDirectory)
    {
        // placeholder
        return Result.Success($"Moved to {publishedMediaLocalDirectory.FullPath}");
    }
}
