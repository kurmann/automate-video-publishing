using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class WriteMetadataToTextFileCommand : ICommand<FileInfo>
{
    private readonly Mpeg4MetadataReadCommand _readCommand;
    private readonly Subject<FileInfo> _broadcaster = new Subject<FileInfo>();

    public IObservable<FileInfo> WhenDataAvailable => _broadcaster.AsObservable();

    public WriteMetadataToTextFileCommand(Mpeg4MetadataReadCommand readCommand) => _readCommand = readCommand;

    public void Execute(WorkflowContext context)
    {
        _readCommand.WhenDataAvailable
            .Subscribe(
                result =>
                {
                    WriteLinesAsync(result.FileInfo, result.Lines)
                        .ContinueWith(task =>
                        {
                            if (task.IsFaulted)
                            {
                                _broadcaster.OnError(task.Exception ?? new Exception("Error on writing lines to TXT file"));
                            }
                            else
                            {
                                _broadcaster.OnNext(result.FileInfo);
                            }
                        });
                },
                exception =>
                {
                    _broadcaster.OnError(exception);
                },
                () =>
                {
                    _broadcaster.OnCompleted();
                }
            );

        _readCommand.Execute(context);
    }

    private static async Task WriteLinesAsync(FileInfo fileInfo, IReadOnlyList<string> lines)
    {
        using var fileStream = fileInfo.Open(FileMode.Append, FileAccess.Write);
        using var streamWriter = new StreamWriter(fileStream);

        foreach (var line in lines)
        {
            await streamWriter.WriteLineAsync(line);
        }
    }
}
