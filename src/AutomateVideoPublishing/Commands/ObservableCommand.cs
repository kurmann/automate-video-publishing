// Die IObservableCommand<T>-Schnittstelle erweitert die IObservable<T>-Schnittstelle 
// um die Fähigkeit, einen Workflow-Kontext auszuführen. Sie definiert zwei Methoden:
// 1. Execute: führt den Workflow-Kontext aus und gibt das Ergebnis als Result<T>-Objekt zurück.
// 2. Subscribe: ermöglicht es Observers, sich bei diesem Observable zu registrieren.
public interface IObservableCommand<T> : IObservable<T>
{
    Task<Result<T>> Execute(WorkflowContext context);
}
