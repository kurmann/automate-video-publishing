// Die Unsubscriber<T>-Klasse dient als Hilfsklasse, um die Beziehung zwischen einem 
// Observable und einem Observer zu beenden. Sie implementiert das IDisposable-Interface, 
// damit sie in einem using-Block oder durch Aufruf der Dispose-Methode verwendet werden kann.
public class Unsubscriber<T> : IDisposable
{
    // Wir speichern eine Referenz auf die Liste der Observer, die beim Observable registriert sind.
    private List<IObserver<T>> _observers;

    // Wir speichern auch eine Referenz auf den Observer, den wir entfernen möchten.
    private IObserver<T> _observer;

    // Im Konstruktor speichern wir die Referenzen auf die Observer-Liste und den spezifischen Observer.
    public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
    {
        _observers = observers;
        _observer = observer;
    }

    // Wenn Dispose aufgerufen wird (entweder manuell oder automatisch durch einen using-Block), 
    // entfernen wir den Observer aus der Liste. Dadurch wird die Beziehung zwischen dem Observable 
    // und diesem Observer beendet.
    public void Dispose()
    {
        if (_observers.Contains(_observer))
            _observers.Remove(_observer);
    }
}