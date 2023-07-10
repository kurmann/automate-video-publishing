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
            var atomicParsleyManager = new AtomicParsleyManager();
            var lines = await atomicParsleyManager.RunAsync(quickTimeMasterFile.Value.FullName);

            return new ReadMasterfileMetadataCommandResult
            {
                // Title = string.IsNullOrWhiteSpace(tfile.Tag.Title) ? Maybe<string>.None : tfile.Tag.Title,
                // Description = string.IsNullOrWhiteSpace(tfile.Tag.Description) ? Maybe<string>.None : tfile.Tag.Description,
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