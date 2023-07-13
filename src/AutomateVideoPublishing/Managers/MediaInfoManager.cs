using System.Diagnostics;
using System.Text;

namespace AutomateVideoPublishing.Managers;

public class MediaInfoManager
{
    private Logger _logger;

    private readonly string _mediaInfoPath = "mediainfo";

    public MediaInfoManager() => _logger = LogManager.GetCurrentClassLogger();

    public async Task<List<string>> RunAsync(string filePath)
    {
        _logger.Info($"Running MediaInfoManager for file: {filePath}");

        var arguments = $"--BOM -f \"{filePath}\"";
        _logger.Info($"Command: {_mediaInfoPath} {arguments}");
        var lines = new List<string>();

        var psi = new ProcessStartInfo(_mediaInfoPath, arguments)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        using (var process = new Process { StartInfo = psi, EnableRaisingEvents = true })
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    _logger.Info("Received line: {Line}", e.Data);
                    lines.Add(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
        }

        return lines;
    }

}