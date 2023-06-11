using AutomateVideoPublishing.Entities;
using CommandLine;

namespace AutomateVideoPublishing
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ReadOptions, TransmitOptions>(args)
                .WithParsed<ReadOptions>(opts => RunReadMetadataCommand(opts.File))
                .WithParsed<TransmitOptions>(opts => RunTransmitMetadataCommand(opts.SourceFile, opts.TargetFile));
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

        public static void RunTransmitMetadataCommand(string? sourceFile, string? targetFile)
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

    [Verb("read", HelpText = "Read metadata from file.")]
    public class ReadOptions
    {
        [Option('f', "file", Required = true, HelpText = "File to process.")]
        public string? File { get; set; }
    }

    [Verb("transmit", HelpText = "Transmit metadata between files.")]
    public class TransmitOptions
    {
        [Option('s', "source", Required = true, HelpText = "Source file.")]
        public string? SourceFile { get; set; }

        [Option('t', "target", Required = true, HelpText = "Target file.")]
        public string? TargetFile { get; set; }
    }
}
