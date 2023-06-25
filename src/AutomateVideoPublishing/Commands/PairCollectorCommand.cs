using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

public class PairCollectorCommand : ICommand<QuickTimeMpeg4MetadataPair>
{
    private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
    private readonly Subject<QuickTimeMpeg4MetadataPair> _broadcaster = new();

    public IObservable<QuickTimeMpeg4MetadataPair> WhenDataAvailable => _broadcaster.AsObservable();

    public PairCollectorCommand(QuickTimeMetadataReadCommand quickTimeCommand) => _quickTimeCommand = quickTimeCommand;

    public void Execute(WorkflowContext context)
    {
        _quickTimeCommand.WhenDataAvailable
            .SelectMany(container =>
            {
                var targetDirectoryPath = context.PublishedMpeg4Directory.Directory.FullName;
                var groupResult = QuickTimeToMpeg4VersionsMetadataGroup.Create(container.FileInfo.FullName, targetDirectoryPath);

                if (groupResult.IsFailure)
                {
                    _broadcaster.OnError(new Exception(groupResult.Error));
                    return new List<QuickTimeMpeg4MetadataPair>().ToObservable();
                }

                var pairs = new List<QuickTimeMpeg4MetadataPair>();

                foreach (var mpeg4MetadataContainer in groupResult.Value.Mpeg4MetadataContainers)
                {
                    var pairResult = QuickTimeMpeg4MetadataPair.Create(container.FileInfo.FullName, mpeg4MetadataContainer.FileInfo.DirectoryName);

                    if (pairResult.IsFailure)
                    {
                        _broadcaster.OnError(new Exception(pairResult.Error));
                    }
                    else
                    {
                        pairs.Add(pairResult.Value);
                    }
                }

                return pairs.ToObservable();
            })
            .Subscribe(_broadcaster.OnNext);

        _quickTimeCommand.Execute(context);
        _broadcaster.OnCompleted();
    }
}
