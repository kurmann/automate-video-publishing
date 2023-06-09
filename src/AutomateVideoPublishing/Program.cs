using AutomateVideoPublishing.Entities;
using AutomateVideoPublishing.Models;
using CommandLine;
using CSharpFunctionalExtensions;

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
            var formattedUnicodeJsonResult = QuickTimeMetadataContainer.Create(opts.File)
                .Bind(quickTimeMetadataContainer => FormattedUnicodeJson.Create(quickTimeMetadataContainer.RawMetadata));
            
            if (formattedUnicodeJsonResult.IsFailure)
            {
                Console.WriteLine($"Error on trying to get QuickTime metadata: {formattedUnicodeJsonResult.Error}");
            }

            Console.WriteLine(formattedUnicodeJsonResult.Value);
        }
    }

}
