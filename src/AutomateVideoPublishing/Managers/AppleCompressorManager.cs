using System.Diagnostics;
using AutomateVideoPublishing.Entities.AppleCompressor;

namespace AutomateVideoPublishing.Managers;

public class AppleCompressorManager
{
    private readonly string _compressorPath = "/Applications/Compressor.app/Contents/MacOS/Compressor";
    private readonly Logger _logger;

    public AppleCompressorManager() => _logger = LogManager.GetCurrentClassLogger();

    public async Task<Result<FileInfo>> RunAsync(AppleCompressorSettings settings)
    {
        // Manually creating the setting path
        var profilePath = $"~/Library/Application Support/Compressor/Settings/{settings.ProfileName}.cmprstng";

        string masterFilePath = settings.MasterFile.FullName;
        var outputFileName = Path.ChangeExtension(Path.GetFileName(masterFilePath), ".m4v");
        var outputFilePath = Path.Combine(settings.OutputDirectory, outputFileName);

        var arguments = $"-batchname \"Batch von {Path.GetFileName(masterFilePath)}\" -jobpath \"{masterFilePath}\" -settingpath \"{profilePath}\" -locationpath \"{outputFilePath}\"";

        _logger.Info($"Compressor-Befehl ausführen für Datei: {masterFilePath}");
        _logger.Info($"Command: {_compressorPath} {arguments}");

        var psi = new ProcessStartInfo(_compressorPath, arguments)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = new Process { StartInfo = psi, EnableRaisingEvents = true })
        {
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    _logger.Info("Erhaltene Zeile: {Line}", e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            await process.WaitForExitAsync();
        }

        var compressedFile = new FileInfo(outputFilePath);
        return compressedFile.Exists
            ? Result.Success(compressedFile)
            : Result.Failure<FileInfo>($"Compressed file was not created: {outputFilePath}");
    }
}
