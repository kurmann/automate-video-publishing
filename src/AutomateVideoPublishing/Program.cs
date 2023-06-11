using AutomateVideoPublishing.Entities;
using CommandLine;

namespace AutomateVideoPublishing;

class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => RunCommand(opts));
    }

    static void RunCommand(Options opts)
    {
        if (opts.ReadMetadata)
        {
            RunReadMetadataCommand(opts.File);
        }
        else if (opts.TransmitMetadata != null && opts.TransmitMetadata.Count() > 1)
        {
            RunTransmitMetadataCommand(opts.TransmitMetadata.First(), opts.TransmitMetadata.Skip(1).First());
        }
    }

    public static void RunReadMetadataCommand(string? file)
    {
        var jsonResult = MediaMetadataJson.Create(file);
        if (jsonResult.IsFailure)
        {
            Console.WriteLine($"Error reading metadata: {jsonResult.Error}");
            return;
        }

        Console.WriteLine(jsonResult.Value.Json);
    }

    public static void RunTransmitMetadataCommand(string sourceFile, string targetFile)
    {
        var tfile = TagLib.File.Create(targetFile);
        string title = tfile.Tag.Description;
        TimeSpan duration = tfile.Properties.Duration;
        Console.WriteLine("Description: {0}, duration: {1}", title, duration);

        // change title in the file
        tfile.Tag.Description = "my new description";
        tfile.Save();
    }
}

public class Options
{
    [Option('r', "read", Required = false, HelpText = "Read metadata from file.")]
    public bool ReadMetadata { get; set; }

    [Option('f', "file", Required = false, HelpText = "File to process.")]
    public string? File { get; set; }

    [Option('t', "transmit-metadata", Required = false, HelpText = "Transmit metadata between files.")]
    public IEnumerable<string>? TransmitMetadata { get; set; }
}
