using AutomateVideoPublishing.Managers;
using AutomateVideoPublishing.Entities.AppleCompressor;

namespace AutomateVideoPublishing.Commands
{
    /// <summary>
    /// Komprimiert die Masterdatei und speichert sie im angegebenen Ausgabeverzeichnis unter Verwendung der angegebenen Compressor-Einstellungen.
    /// </summary>
    public class CompressMasterfileCommand : IAsyncCommand<string, Result<FileInfo>>
    {
        private readonly ILogger _logger;
        private readonly AppleCompressorManager _manager;
        private readonly string _outputDirectory;
        private readonly string _profileName;

        public CompressMasterfileCommand(AppleCompressorManager appleCompressorManager, string outputDirectory, string profileName = "4K")
        {
            _logger = LogManager.GetCurrentClassLogger();
            _manager = appleCompressorManager;
            _outputDirectory = outputDirectory;
            _profileName = profileName;
        }

        public async Task<Result<FileInfo>> ExecuteAsync(string masterfilePath)
        {
            var compressorSettingsResult = AppleCompressorSettings.Create(masterfilePath, _outputDirectory, _profileName);

            if (compressorSettingsResult.IsFailure)
            {
                return Result.Failure<FileInfo>(compressorSettingsResult.Error);
            }

            var compressedFilePath = await _manager.RunAsync(compressorSettingsResult.Value);

            if (compressedFilePath.IsFailure)
            {
                return Result.Failure<FileInfo>(compressedFilePath.Error);
            }

            var compressedFile = compressedFilePath.Value;
            return compressedFile.Exists
                ? Result.Success(compressedFile)
                : Result.Failure<FileInfo>($"Compressed file was not created: {compressedFilePath.Value}");
        }
    }
}
