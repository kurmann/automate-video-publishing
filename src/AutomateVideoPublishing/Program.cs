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
            RunReadMetaDataCommand(opts.File);
        }
    }

    public static void RunReadMetaDataCommand(string? file)
    {
        var fileInfoContainerResult = MediaFileInfoContainer.Create(file);   
        if (fileInfoContainerResult.IsFailure)
        {
            Console.WriteLine($"Error on reading file: {file}");
            return;
        }

        // quicktime and mp4 have different tools to read metadata from
        switch (fileInfoContainerResult.Value.MediaType)
        {
            case MediaType.QuickTimeMov:
                QuickTimeMetadataContainer.Create(fileInfoContainerResult.Value)
                    .Bind(quickTimeMetadataContainer => FormattedUnicodeJson.Create(quickTimeMetadataContainer.RawMetadata)
                    .Tap(formattedUnicodeJson => Console.WriteLine(formattedUnicodeJson)))
                    .TapError(error => Console.WriteLine($"Error on trying to get QuickTime metadata: {error}"));
                break;
            case MediaType.Mpeg4:
                Console.WriteLine("Here would the mp4 metadata be read.");
                break;
            default:
                Console.WriteLine("Cannot unsupported file");
                break;
        }
    }
}
