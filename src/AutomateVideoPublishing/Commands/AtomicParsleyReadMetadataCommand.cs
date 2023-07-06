using AutomateVideoPublishing.Managers;
using AutomateVideoPublishing.Entities.AtomicParsley;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;

namespace AutomateVideoPublishing.Commands
{
    public class AtomicParsleyReadMetadataCommand
    {
        private readonly Subject<AtomicParsleyMetadataReadOutputLine> _subject = new();
        private readonly ProcessManager _processManager;
        private readonly string _atomicParsleyPath = "AtomicParsley";

        public AtomicParsleyReadMetadataCommand() => _processManager = new ProcessManager();

        public IObservable<AtomicParsleyMetadataReadOutputLine> Lines => _subject.AsObservable();

        public void Run(string filePath)
        {
            var arguments = new AtomicParsleyArguments()
                                .AddFilePath(filePath)
                                .AddOption("-t");

            var outputObserver = Observer.Create<string>(
                onNext: line => _subject.OnNext(AtomicParsleyMetadataReadOutputLine.Create(line)),
                onCompleted: () => _subject.OnCompleted()
            );
            _processManager.StartNewProcess(_atomicParsleyPath, arguments.ToString(), outputObserver);
        }
    }
}
