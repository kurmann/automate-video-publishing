public class ReadAllMetadataStrategyObserver : WorkflowStrategyObserver<FileInfo>
{
    public ReadAllMetadataStrategyObserver() : base(nameof(ReadAllMetadataStrategy)) { }

    public override void OnNext(FileInfo value) => Broadcast($"New Json file created: {value.FullName}");
}
