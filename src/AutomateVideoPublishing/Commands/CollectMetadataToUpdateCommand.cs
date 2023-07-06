using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands
{
    public class CollectMetadataToUpdateCommand : ICommand<UpdateMetadataBaseData>
    {
        private readonly Subject<UpdateMetadataBaseData> _broadcaster = new();
        private readonly Mpeg4MetadataReadCommand _mpeg4MetadataReadCommand;

        public IObservable<UpdateMetadataBaseData> WhenDataAvailable => _broadcaster.AsObservable();

        public CollectMetadataToUpdateCommand(Mpeg4MetadataReadCommand mpeg4MetadataReadCommand) 
            => _mpeg4MetadataReadCommand = mpeg4MetadataReadCommand;

        public void Execute(WorkflowContext context)
        {
            _mpeg4MetadataReadCommand.WhenDataAvailable.Subscribe(onNext: metadataReadResult =>
            {
                var metadataCollectionResult = Mpeg4MetadataCollection.Create(metadataReadResult.Lines);
                if (metadataCollectionResult.IsFailure)
                {
                    var exception = new Exception($"Error on parsing AtomicParsley output to metadata dictionary: {metadataCollectionResult.Error}");
                    _broadcaster.OnError(exception);
                    return;
                }
                var metadataCollection = metadataCollectionResult.Value;

                string? description = metadataCollection.Metadata.GetValueOrDefault("©des");
                var baseDataResult = UpdateMetadataBaseData.Create(description, metadataReadResult.FileInfo.FullName);

                if (baseDataResult.IsFailure)
                {
                    _broadcaster.OnError(new Exception($"Error on preparing base data: {baseDataResult.Error}"));
                    return;
                }

                _broadcaster.OnNext(baseDataResult.Value);
                _broadcaster.OnCompleted();  // Signal completion after sending the value
            });

            _mpeg4MetadataReadCommand.Execute(context);
        }
    }
}
