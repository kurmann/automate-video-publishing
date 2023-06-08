using CommandLine;

namespace AutomateVideoPublishing.Models;

public class Options
{
    [Option('r', "read-metadata", Required = false, HelpText = "Liest Metadaten aus der angegebenen Datei.")]
    public bool ReadMetadata { get; set; }

    [Value(0, MetaName = "file", HelpText = "Dateipfad.", Required = true)]
    public string? File { get; set; }
}
