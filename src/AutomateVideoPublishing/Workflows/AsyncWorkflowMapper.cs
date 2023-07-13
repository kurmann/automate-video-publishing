namespace AutomateVideoPublishing.Workflows;

public class AsyncWorkflowMapper
{

    public IAsyncWorkflow SelectedWorkflow { get; private set; }

    private AsyncWorkflowMapper(IAsyncWorkflow workflow) => SelectedWorkflow = workflow;

    public static Result<IAsyncWorkflow> Create(string? workflowName)
    {
        if (string.IsNullOrWhiteSpace(workflowName))
        {
            // todo: read metadata as default workflow
        }

        IAsyncWorkflow workflow;
        switch (workflowName)
        {
            case nameof(SingleVideoPublishStrategy):
                workflow = new SingleVideoPublishStrategy();
                break;
            default:
                return Result.Failure<IAsyncWorkflow>($"Unknown strategy: {workflowName}");
        }

        return Result.Success<IAsyncWorkflow>(workflow);
    }
}
