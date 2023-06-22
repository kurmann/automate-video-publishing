using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands
{
    public class MetadataTransferCommand : ICommand<TransferredMetadata>
    {
        private readonly QuickTimeMetadataReadCommand _quickTimeCommand;
        private readonly Subject<TransferredMetadata> _broadcaster = new();

        public IObservable<TransferredMetadata> WhenDataAvailable => _broadcaster.AsObservable();

        public MetadataTransferCommand(QuickTimeMetadataReadCommand quickTimeCommand) => _quickTimeCommand = quickTimeCommand;

        public void Execute(WorkflowContext context)
        {
            _quickTimeCommand.WhenDataAvailable.Subscribe(container =>
            {
                // Hier ist ein Platzhalter für die Suche nach einer gleichnamigen Datei im Zielverzeichnis 
                // und die Übertragung der Metadaten. Bitte ersetzen Sie diesen Code durch Ihren eigenen Code.
                var transferredMetadata = new TransferredMetadata(container.FileInfo.Name, "description", "year");
                _broadcaster.OnNext(transferredMetadata);
            });

            _quickTimeCommand.Execute(context);

            _broadcaster.OnCompleted();
        }
    }

    public class TransferredMetadata
    {
        public string FileName { get; }
        public string Description { get; }
        public string Year { get; }

        public TransferredMetadata(string fileName, string description, string year)
        {
            FileName = fileName;
            Description = description;
            Year = year;
        }
    }
}
