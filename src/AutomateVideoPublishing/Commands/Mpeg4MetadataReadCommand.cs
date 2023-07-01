using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class Mpeg4MetadataReadCommand : ICommand<string>
{
    private readonly Subject<string> _broadcaster = new();
    public IObservable<string> WhenDataAvailable => _broadcaster.AsObservable();

    public Mpeg4MetadataReadCommand() { }

    public void Execute(WorkflowContext context)
    {
        foreach (var fileInfo in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var atomicParsleyReadCommand = new AtomicParsleyCommand(fileInfo.FullName).ForReading().WithMetadata();
            var consoleOutput = RunAtomicParsley(atomicParsleyReadCommand);
            _broadcaster.OnNext(consoleOutput);
        }

        _broadcaster.OnCompleted();
    }

    private string RunAtomicParsley(AtomicParsleyReadCommand atomicParsleyReadCommand)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = $"-c \"{atomicParsleyReadCommand.ToString().Replace("\"", "\\\"")}\""
        };

        var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new Exception("Initialized process for AtomicParsley is null");
        }
        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(error))
        {
            var exception = new Exception($"AtomicParsley execution error: {error}");
            _broadcaster.OnError(exception);
        }

        return output;
    }
}
