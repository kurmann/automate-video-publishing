using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Klasse um Metadaten aller Dateien im MPEG4-Verzeichnis des Workflow-Context auszulesen.
/// </summary>
public class Mpeg4MetadataReadCommand : ICommand<AtomicParsleyMetadataReadResult>
{
    private readonly Subject<AtomicParsleyMetadataReadResult> _broadcaster = new();
    private readonly AtomicParsleyReadMetadataCommand command;

    public IObservable<AtomicParsleyMetadataReadResult> WhenDataAvailable => _broadcaster.AsObservable();

    public Mpeg4MetadataReadCommand(AtomicParsleyReadMetadataCommand command) => this.command = command;

    public void Execute(WorkflowContext context)
    {
        int fileCount = context.PublishedMpeg4Directory.Mpeg4Files.Count();
        int processedFiles = 0;

        foreach (var fileInfo in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var readResult = AtomicParsleyMetadataReadResult.Create(fileInfo);

            command.Lines.Subscribe(
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
            command.Run(fileInfo.FullName);
        }
    }
}

