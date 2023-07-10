using AutomateVideoPublishing.Managers;

namespace AutomateVideoPublishing.Commands;

public class ReadMasterfileMetadataCommand : ICommand<string, Result<ReadMasterfileMetadataCommandResult>>
{
    private Logger _logger;

    public ReadMasterfileMetadataCommand() => _logger = LogManager.GetCurrentClassLogger();

    public Task<Result<ReadMasterfileMetadataCommandResult>> ExecuteAsync(string masterfilePath) => 
        QuickTimeMasterFile.Create(masterfilePath)
            .Bind(master => GetMetadata(master)
            .Tap(result => Task.FromResult(result)));

    private static async Task<Result<ReadMasterfileMetadataCommandResult>> GetMetadata(QuickTimeMasterFile quickTimeMasterFile)
    {
        try
        {
            var manager = new MediaInfoManager();
            var lines = (await manager.RunAsync(quickTimeMasterFile.Value.FullName));
            var metadataOutputResult = MediaInfoMetadataOutput.Create(lines);
            if (metadataOutputResult.IsFailure)
            {
                return Result.Failure<ReadMasterfileMetadataCommandResult>(metadataOutputResult.Error);
            }

            return new ReadMasterfileMetadataCommandResult
            {
                Title = metadataOutputResult.Value.Title,
                Description = metadataOutputResult.Value.Description
            };
        }
        catch (Exception ex)
        {
            return Result.Failure<ReadMasterfileMetadataCommandResult>($"Error on reading metdata with TagLib#: {ex.Message}");
        }
    }
}

public class ReadMasterfileMetadataCommandResult
{
    public Maybe<string> Title { get; set; }
    public Maybe<string> Description { get; set; }
}