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
            var fileInfoResult = FileInfoContainer.Create(opts.File);

            fileInfoResult
                .Check(fileInfoContainer => QuickTimeMetadataContainer.Create(fileInfoContainer)
                .Bind(quickTimeMetadataContainer => FormattedUnicodeJson.Create(quickTimeMetadataContainer.RawMetadata))
                .Tap(formattedJson => Console.WriteLine(formattedJson))
                .TapError(error => Console.WriteLine($"Error on trying to get QuickTime metadata: {error}")));
        }
    }

}
