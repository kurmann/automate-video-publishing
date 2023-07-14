using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace AutomateVideoPublishing.Managers;

public class MediaInfoManager
{
    private Logger _logger;

    private readonly string _mediaInfoPath = "mediainfo";

    public MediaInfoManager() => _logger = LogManager.GetCurrentClassLogger();

    public async Task<Result<JsonDocument>> RunAsync(string filePath)
    {
        _logger.Info($"Running MediaInfoManager for file: {filePath}");

        var arguments = $"--Output=JSON -f \"{filePath}\"";
        _logger.Info($"Command: {_mediaInfoPath} {arguments}");
        var lines = new List<string>();

        var psi = new ProcessStartInfo(_mediaInfoPath, arguments)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.Latin1
        };

        using (var process = new Process { StartInfo = psi, EnableRaisingEvents = true })
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    _logger.Info(e.Data);
                    lines.Add(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
        }

        var jsonString = string.Join("", lines);
        try
        {
            return JsonDocument.Parse(jsonString);
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "Failed to parse JSON output from MediaInfo");
            return Result.Failure<JsonDocument>($"Failed to parse JSON output from MediaInfo: {ex.Message}");
        }
    }


}