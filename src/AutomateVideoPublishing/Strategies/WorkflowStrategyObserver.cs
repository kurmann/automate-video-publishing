namespace AutomateVideoPublishing.Strategies;

/// <summary>
/// Die Klasse WorkflowStrategyObserver ist eine Kombination aus Observer und Observable. 
/// Als Observer hört sie auf Ereignisse, die durch den beobachteten Prozess ausgelöst werden 
/// (in diesem Fall die WorkflowStrategy). Als Observable ermöglicht sie es anderen Objekten, 
/// sie zu beobachten und Benachrichtigungen zu erhalten, wenn bestimmte Ereignisse auftreten.
/// </summary>
/// <typeparam name="T">Der Typ, der bei der OnNext-Methode verwendet wird.</typeparam>
public abstract class WorkflowStrategyObserver<T> : IObserver<T>, IObservable<string>
{
    // Liste aller Observer, die auf Ereignisse dieser Klasse warten
    private List<IObserver<string>> observers = new List<IObserver<string>>();
    
    // Name der Strategie, wird für Meldungen verwendet
    protected string StrategyName { get; }

    // Konstruktor, erfordert den Namen der Strategie
    protected WorkflowStrategyObserver(string strategyName) => StrategyName = strategyName;

    // Methode, die aufgerufen wird, wenn das beobachtete Objekt ein neues Ereignis meldet
    // In dieser Basisklasse wird sie nicht implementiert, kann aber von erbenden Klassen überschrieben werden
    public virtual void OnNext(T value)
    {
        // Standardmäßig passiert nichts, wenn OnNext aufgerufen wird.
        // Unterklassen können diese Methode überschreiben, um spezifisches Verhalten zu definieren.
    }

    // Methode, die aufgerufen wird, wenn das beobachtete Objekt einen Fehler meldet
    // Sendet eine Nachricht an alle beobachtenden Objekte dieser Klasse
    public void OnError(Exception error) => Broadcast($"Ein Fehler ist aufgetreten in {StrategyName}: {error.Message}");

    // Methode, die aufgerufen wird, wenn das beobachtete Objekt seine Aufgabe abgeschlossen hat
    // Sendet eine Nachricht an alle beobachtenden Objekte dieser Klasse
    public void OnCompleted() => Broadcast($"{StrategyName} hat seine Aufgaben abgeschlossen.");

    // Methode, die es anderen Objekten ermöglicht, diese Klasse zu beobachten
    // Fügt den Observer zur Liste der beobachtenden Objekte hinzu, falls er noch nicht vorhanden ist
    public IDisposable Subscribe(IObserver<string> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }

        return new Unsubscriber<string>(observers, observer);
    }

    // Hilfsmethode zum Versenden von Nachrichten an alle beobachtenden Objekte
    protected void Broadcast(string message)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(message);
        }
    }
}
