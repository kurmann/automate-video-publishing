using System.Reactive.Linq;
using System.Reactive.Subjects;
using AutomateVideoPublishing.Commands;

namespace AutomateVideoPublishing.Strategies;

public class LocalVideoPublishStrategy : IWorkflowStrategy
{
    private Subject<string> _broadcaster = new();

    public IObservable<string> WhenStatusUpdateAvailable => _broadcaster.AsObservable();

    public void Execute(WorkflowContext context)
    {
        var mpeg4MetadataReadCommand = new Mpeg4MetadataReadCommand();

        mpeg4MetadataReadCommand.WhenDataAvailable.Subscribe(
            consoleOutput =>
            {
                if (consoleOutput != null)
                {
                    _broadcaster.OnNext(consoleOutput);
                }
            },
            exception =>
            {
                // Handle any error
                _broadcaster.OnError(exception);
            }
        );

        mpeg4MetadataReadCommand.Execute(context);
    }
}
