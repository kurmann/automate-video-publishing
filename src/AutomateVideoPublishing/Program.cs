using AutomateVideoPublishing.Models;
using AutomateVideoPublishing.Services;
using CommandLine;

namespace AutomateVideoPublishing;

partial class Program
{
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => RunCommand(opts));
    }

    static void RunCommand(Options opts)
    {
        if (opts.File == null)
        {
            Console.WriteLine("Datei-Parameter wurde nicht angegeben.");
            return;
        }

        if (opts.ReadMetadata)
        {
            var metadataService = new MetadataService();
            metadataService.ReadQuicktimeMetadata(opts);
        }
    }
}
