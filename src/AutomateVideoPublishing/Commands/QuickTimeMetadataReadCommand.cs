using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class QuickTimeMetadataReadCommand : ICommand<QuickTimeMetadataContainer>
{
    private readonly Subject<QuickTimeMetadataContainer> _metadataAvailable = new();

    public IObservable<QuickTimeMetadataContainer> WhenDataAvailable => _metadataAvailable.AsObservable();

    public void Execute(WorkflowContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        foreach (var quickTimeFile in context.QuickTimeMasterDirectory.QuickTimeFiles)
        {
            var containerResult = QuickTimeMetadataContainer.Create(quickTimeFile.FullName);
            if (containerResult.IsFailure)
            {
                _metadataAvailable.OnError(new Exception(containerResult.Error));
                return;
            }

            _metadataAvailable.OnNext(containerResult.Value);
        }

        _metadataAvailable.OnCompleted();
    }
}
