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
                            if (task.Result.IsFailure)
                            {
                                _broadcaster.OnError(new Exception(task.Result.Error));
                            }
                            else
                            {
                                _broadcaster.OnNext(task.Result.Value);
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


    private static async Task<Result<FileInfo>> WriteLinesAsync(FileInfo fileInfo, IReadOnlyList<string> lines)
    {
        try
        {
            // test
            var metadata = Mpeg4MetadataCollection.Create(lines);

            var txtFileName = Path.ChangeExtension(fileInfo.FullName, ".txt");
            using var fileStream = new FileStream(txtFileName, FileMode.Create, FileAccess.Write);
            using var streamWriter = new StreamWriter(fileStream);

            foreach (var line in lines)
            {
                await streamWriter.WriteLineAsync(line);
            }

            await streamWriter.FlushAsync();

            return Result.Success(new FileInfo(txtFileName));
        }
        catch (Exception ex)
        {
            return Result.Failure<FileInfo>(ex.Message);
        }
    }
}