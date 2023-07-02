using AutomateVideoPublishing.Managers;
using AutomateVideoPublishing.Entities.AtomicParsley;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands
{
    public class AtomicParsleyReadMetadataCommand
    {
        private readonly Subject<string> _subject = new Subject<string>();
        private readonly ProcessManager _processManager;
        private readonly string _atomicParsleyPath = "AtomicParsley";

        public AtomicParsleyReadMetadataCommand() => _processManager = new ProcessManager();

        public IObservable<string> Lines => _subject.AsObservable();

        public void Run(string filePath)
        {
            var arguments = new AtomicParsleyArguments()
                                .AddFilePath(filePath)
                                .AddOption("-t");

            _processManager.StartNewProcess(_atomicParsleyPath, arguments.Build, _subject);
        }
    }
}
