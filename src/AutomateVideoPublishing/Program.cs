using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Models;
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
    }

    public static void RunReadMetadataCommand(string? file)
    {
        var jsonResult = MediaMetadataJson.Create(file);
        if (jsonResult.IsFailure)
        {
            Console.WriteLine($"Error reading metadata from file: {jsonResult.Value}");
            return;
        }

        Console.WriteLine(jsonResult.Value.Json);
    }
}
