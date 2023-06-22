using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace AutomateVideoPublishing.Commands;

public class WriteJsonMetadataCommand : ICommand<string>
{
    private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
    private readonly Mpeg4DirectoryMetadataReadCommand _mpeg4Command;
    private readonly Subject<string> _broadcaster = new Subject<string>();

    public IObservable<string> WhenDataAvailable => _broadcaster.AsObservable();

    public WriteJsonMetadataCommand(QuickTimeMetadataReadCommand quickTimeCommand, Mpeg4DirectoryMetadataReadCommand mpeg4Command)
    {
        _quickTimeCommand = quickTimeCommand;
        _mpeg4Command = mpeg4Command;
    }

    public void Execute(WorkflowContext context)
    {
        // Merge the data streams from both commands into one
        _quickTimeCommand.WhenDataAvailable
            .SelectMany(container => WriteJsonAsync(container.FileInfo).ToObservable())
            .Merge(_mpeg4Command.WhenDataAvailable.SelectMany(container => WriteJsonAsync(container.FileInfo).ToObservable()))
            .Subscribe(_broadcaster);

        _quickTimeCommand.Execute(context);
        _mpeg4Command.Execute(context);

        _broadcaster.OnCompleted();
    }

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
            _broadcaster.OnError(new Exception($"Json file from file {fileInfo} could not be generated."));
            return string.Empty;
        }
    }
}
