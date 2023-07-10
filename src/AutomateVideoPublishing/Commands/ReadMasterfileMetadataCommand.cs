namespace AutomateVideoPublishing.Commands;

public class ReadMasterfileMetadataCommand : ICommand<string, Result<ReadMasterfileMetadataCommandResult>>
{
    private Logger _logger;

    public ReadMasterfileMetadataCommand() => _logger = LogManager.GetCurrentClassLogger();

    public Task<Result<ReadMasterfileMetadataCommandResult>> ExecuteAsync(string masterfilePath)
    {       
        _logger.Info("Start executing ReadMasterfileMetadataCommand");

        // Implement async logic here to read metadata using TagLib-Sharp

        _logger.Info("Finished executing ReadMasterfileMetadataCommand");
        var result = Result.Success<ReadMasterfileMetadataCommandResult>(new ReadMasterfileMetadataCommandResult());
        return Task.FromResult(result);
    }
}

public class ReadMasterfileMetadataCommandResult 
{

}