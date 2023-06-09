using AutomateVideoPublishing.Models;
using AutomateVideoPublishing.Services;
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
        if (opts.File == null)
        {
            Console.WriteLine("Datei-Parameter wurde nicht angegeben.");
            return;
        }

        if (opts.ReadMetadata)
        {
            if (opts.File == null)
            {
                Console.WriteLine("File-Parameter ist leer.");
                return;
            }
            else
            {
                var metadataService = new MetadataService(opts.File);

                try
                {
                    var quickTimeMetaData = metadataService.TryGetQuickTimeMetadata();
                    Console.WriteLine(JsonService.GetFormattedUnicodeJson(quickTimeMetaData));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on trying to get QuickTime metadata: {ex.Message}");
                    return;
                }
            }

        }
    }
}
