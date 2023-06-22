using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class Mpeg4DirectoryMetadataReadCommand : ICommand<Mpeg4MetadataContainer>
{
    private readonly Subject<Mpeg4MetadataContainer> _broadcaster = new();

    public IObservable<Mpeg4MetadataContainer> WhenDataAvailable => _broadcaster.AsObservable();

    public void Execute(WorkflowContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        foreach (var mpeg4File in context.PublishedMpeg4Directory.Mpeg4Files)
        {
            var containerResult = Mpeg4MetadataContainer.Create(mpeg4File.FullName);
            if (containerResult.IsFailure)
            {
                _broadcaster.OnError(new Exception(containerResult.Error));
                return;
            }

            _broadcaster.OnNext(containerResult.Value);
        }

        _broadcaster.OnCompleted();
    }
}
