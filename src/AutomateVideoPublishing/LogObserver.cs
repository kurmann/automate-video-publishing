using NLog;

public class LogObserver : IObserver<string>
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

    public void OnNext(string value)
    {
        Logger.Info(value);
    }

    public void OnError(Exception error)
    {
        Logger.Error(error, "An error occurred");
    }

    public void OnCompleted()
    {
        Logger.Info("Finished logging");
    }
}
