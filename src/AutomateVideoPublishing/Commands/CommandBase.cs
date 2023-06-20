public abstract class CommandBase : ICommand
{
    public abstract Task Execute();

    public async Task StartExecute()
    {
        // Call Execute and await its completion
        await Execute();
    }
}
