using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Klasse um Metadaten aller Dateien im MPEG4-Verzeichnis des Workflow-Context auszulesen.
/// </summary>
public class Mpeg4MetadataReadCommand : ICommand<AtomicParsleyMetadataReadResult>
{
    private readonly Subject<AtomicParsleyMetadataReadResult> _broadcaster = new();
    private readonly AtomicParsleyReadMetadataCommand atomicParsleyCommand;

    public IObservable<AtomicParsleyMetadataReadResult> WhenDataAvailable => _broadcaster.AsObservable();

    public Mpeg4MetadataReadCommand(AtomicParsleyReadMetadataCommand atomicParsleyRunCommand) => this.atomicParsleyCommand = atomicParsleyRunCommand;

    public void Execute(WorkflowContext context)
    {
        int fileCount = context.PublishedMpeg4Directory.Mpeg4Files.Count();
        int processedFiles = 0;

        foreach (var fileInfo in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var readResult = AtomicParsleyMetadataReadResult.Create(fileInfo);

            atomicParsleyCommand.Lines.Subscribe(
                line => readResult.AddLine(line),
                () =>
                {
                    _broadcaster.OnNext(readResult);
                    processedFiles++;
                    if (processedFiles == fileCount)
                    {
                        _broadcaster.OnCompleted();
                    }
                });
            atomicParsleyCommand.Run(fileInfo.FullName);
        }
    }
}

