using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace AutomateVideoPublishing.Commands;

/// <summary>
/// Führt den AtomicParsley-Befehl aus und übermittelt die Zeilenausgabe an ein Observable.
/// </summary>
public class AtomicParsleyRunCommand
{
    private readonly Subject<string> _subject = new Subject<string>();

    /// <summary>
    /// Observable, das die Zeilenausgabe von AtomicParsley verbreitet.
    /// </summary>
    public IObservable<string> Lines => _subject.AsObservable();

    /// <summary>
    /// Führt den AtomicParsley-Befehl für eine gegebene Datei aus.
    /// </summary>
    /// <param name="filePath">Der Pfad zur Datei, für die AtomicParsley ausgeführt werden soll.</param>
    public void Run(string filePath)
    {
        var psi = new ProcessStartInfo("AtomicParsley", $"\"{filePath}\" -t")
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = psi,
            EnableRaisingEvents = true
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                _subject.OnNext(e.Data);
            }
            else
            {
                // Wenn es keine Daten mehr gibt, schließen wir das Observable.
                _subject.OnCompleted();
            }
        };

        process.Start();
        process.BeginOutputReadLine();
    }
}
