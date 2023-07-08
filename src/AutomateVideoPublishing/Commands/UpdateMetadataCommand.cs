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
    private readonly Subject<UpdateMetadataResult> _broadcaster = new();
    private readonly CollectMetadataToUpdateCommand _collectMetadataToUpdateCommand;
    private readonly ProcessManager _processManager;

    public IObservable<UpdateMetadataResult> WhenDataAvailable => _broadcaster.AsObservable();

    public UpdateMetadataCommand(CollectMetadataToUpdateCommand collectMetadataToUpdateCommand)
    {
        _collectMetadataToUpdateCommand = collectMetadataToUpdateCommand;
        _processManager = new ProcessManager();
    }

    public void Execute(WorkflowContext context)
    {
        _collectMetadataToUpdateCommand.WhenDataAvailable.Subscribe(onNext: metadataBaseData =>
        {
            UpdatedTags updatedTags = new UpdatedTags(metadataBaseData.Date, metadataBaseData.Description);
            if (updatedTags.Date.HasValue)
            {
                var dayArguments = AtomicParsleyUpdateMetadataArguments.CreateOverwriteDay(metadataBaseData.FileInfo.FullName, updatedTags.Date.Value);
                _processManager.StartNewProcess("AtomicParsley", dayArguments.Arguments);
            }

            if (updatedTags.Description.HasValue)
            {
                var descriptionArguments = AtomicParsleyUpdateMetadataArguments.CreateOverwriteDescription(metadataBaseData.FileInfo.FullName, updatedTags.Description.Value);
                _processManager.StartNewProcess("AtomicParsley", descriptionArguments.Arguments);
            }

            var result = UpdateMetadataResult.Create(metadataBaseData.FileInfo, updatedTags, metadataBaseData.MetadataCollection);
            _broadcaster.OnNext(result);
        });

        _collectMetadataToUpdateCommand.Execute(context);
    }
}

public record UpdatedTags(Maybe<DateTime> Date, Maybe<string> Description);

public class UpdateMetadataResult
{
    public FileInfo File { get; }
    public UpdatedTags Tags { get; }
    public Mpeg4MetadataCollection UpdatedMetadata { get; }

    public string SummaryMessage
    {
        get
        {
            var datePart = Tags.Date.HasValue ? $"Date updated to {Tags.Date.Value:yyyy-MM-dd HH:mm:ss}" : "Date not updated";
            var descPart = Tags.Description.HasValue ? $", Description updated to {Tags.Description.Value}" : ", Description not updated";
            return $"In the file {File.FullName}, " + datePart + descPart;
        }
    }

    private UpdateMetadataResult(FileInfo file, UpdatedTags tags, Mpeg4MetadataCollection updatedMetadata)
    {
        File = file;
        Tags = tags;
        UpdatedMetadata = updatedMetadata;
    }

    public static UpdateMetadataResult Create(FileInfo file, UpdatedTags tags, Mpeg4MetadataCollection updatedMetadata)
        => new UpdateMetadataResult(file, tags, updatedMetadata);
}
