using AutomateVideoPublishing.Entities.AtomicParsley;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using NLog;

namespace AutomateVideoPublishing.Managers;

public class AtomicParsleyManager
{
    private readonly Subject<AtomicParsleyMetadataReadOutputLine> _subject = new();
    private readonly ProcessManager _processManager;
    private readonly Logger _logger;
    private readonly string _atomicParsleyPath = "AtomicParsley";

    public AtomicParsleyManager()
    {
        _processManager = new ProcessManager();
        _logger = LogManager.GetCurrentClassLogger();
    }

    public IObservable<AtomicParsleyMetadataReadOutputLine> Lines => _subject.AsObservable();

    public void Run(string filePath)
    {
        _logger.Info("Running AtomicParsleyManager for file: {FilePath}", filePath);

        var arguments = new AtomicParsleyArguments()
                            .AddFilePath(filePath)
                            .AddOption("-t");

        var outputObserver = Observer.Create<string>(
            onNext: line =>
            {
                _logger.Info("Received line: {Line}", line);
                _subject.OnNext(AtomicParsleyMetadataReadOutputLine.Create(line));
            },
            onCompleted: () => _subject.OnCompleted(),
            onError: ex =>
            {
                _logger.Error(ex, "Error occurred during AtomicParsleyManager for file: {FilePath}", filePath);
                _subject.OnError(ex);
            }
        );

        try
        {
            _processManager.StartNewProcess(_atomicParsleyPath, arguments.ToString(), outputObserver);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error while running AtomicParsleyManager for file: {FilePath}", filePath);
            _subject.OnError(ex);
        }
    }
}
