using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class Mpeg4MetadataReadCommand : ICommand<AtomicParsleyMetadataReadResult>
{
    private readonly Subject<AtomicParsleyMetadataReadResult> _broadcaster = new();
    private readonly AtomicParsleyRunCommand atomicParsleyRunCommand;

    public IObservable<AtomicParsleyMetadataReadResult> WhenDataAvailable => _broadcaster.AsObservable();

    public Mpeg4MetadataReadCommand(AtomicParsleyRunCommand atomicParsleyRunCommand) => this.atomicParsleyRunCommand = atomicParsleyRunCommand;

    public void Execute(WorkflowContext context)
    {
        foreach (var fileInfo in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var atomicParsleyMetadataReadResultResult = AtomicParsleyMetadataReadResult.Create(fileInfo);
            if (atomicParsleyMetadataReadResultResult.IsFailure)
            {
                _broadcaster.OnError(new Exception($"Error on preparing AtomicParsley result: {atomicParsleyMetadataReadResultResult.Error}"));
                return;
            }

            var atomicParsleyRunCommand = new AtomicParsleyRunCommand();

            atomicParsleyRunCommand.Lines.Subscribe(onNext: line =>
            {
                atomicParsleyMetadataReadResultResult.Value.AddLine(line);
            }, onCompleted: () =>
            {
                _broadcaster.OnNext(atomicParsleyMetadataReadResultResult.Value);
            });

            atomicParsleyRunCommand.Run(fileInfo.FullName);
        }
    }

}
