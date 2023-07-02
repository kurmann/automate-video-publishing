using System.Diagnostics;

namespace AutomateVideoPublishing.Managers;

public class ProcessManager
{
    public void StartNewProcess(string command, string arguments, IObserver<string> outputObserver)
    {
        var psi = new ProcessStartInfo(command, arguments)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process { StartInfo = psi, EnableRaisingEvents = true };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                outputObserver.OnNext(e.Data);
            }
            else
            {
                outputObserver.OnCompleted();
            }
        };

        process.Start();
        process.BeginOutputReadLine();
        process.WaitForExit();
    }
}
