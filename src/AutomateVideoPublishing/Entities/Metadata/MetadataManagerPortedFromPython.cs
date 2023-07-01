using System;
using System.Diagnostics;
using System.IO;
using CSharpFunctionalExtensions;
using Serilog;

public class MetadataManager
{
    private const string AtomicParsleyPath = @"/usr/local/bin/AtomicParsley";
    private readonly ILogger _logger;

    public MetadataManager(ILogger logger)
    {
        _logger = logger;
    }

    private Result<string, string> RunCommand(string cmd)
    {
        var processInfo = new ProcessStartInfo("bash", $"-c \"{cmd}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process { StartInfo = processInfo };
        process.Start();

        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (!string.IsNullOrWhiteSpace(error))
        {
            _logger.Error($"Error while executing command {cmd}: {error}");
            return Result.Failure<string>(error);
        }

        return Result.Success(output);
    }

    public Result LogMetadata(string metadata)
    {
        _logger.Information("Following MP4 metadata was read from the source file.");
        foreach (var line in metadata.Split(Environment.NewLine))
        {
            _logger.Information(line);
        }

        return Result.Success();
    }
}

public class MetadataNotFoundException : Exception
{
    public MetadataNotFoundException(string message) : base(message) { }
}

public class InvalidDateFormatException : Exception
{
    public InvalidDateFormatException(string message) : base(message) { }
}

public class MetadataReader : MetadataManager
{
    public MetadataReader(ILogger logger) : base(logger) { }

    public Result<string, string> GetMetadata(string filePath)
    {
        var cmd = $"{AtomicParsleyPath} {filePath} -t";
        return RunCommand(cmd);
    }

    private Result<string, string> GetMetadataValue(string metadata, string atom)
    {
        foreach (var line in metadata.Split(Environment.NewLine))
        {
            if (line.StartsWith($"Atom \"{atom}\" contains: "))
            {
                return Result.Success(line.Substring($"Atom \"{atom}\" contains: ".Length).Trim());
            }
        }

        return Result.Failure<string>($"No value for atom '{atom}' found.");
    }

    public Result<string, string> GetDescription(string metadata)
    {
        return GetMetadataValue(metadata, "©des");
    }

    public Result<string, string> GetTvsn(string metadata)
    {
        return GetMetadataValue(metadata, "tvsn");
    }

    public Result<string, string> GetAlbum(string metadata)
    {
        return GetMetadataValue(metadata, "©alb");
    }

    public Result<string, string> GetDay(string metadata)
    {
        var dayResult = GetMetadataValue(metadata, "©day");

        if (dayResult.IsFailure)
        {
            return dayResult;
        }

        var day = dayResult.Value;
        return day.Contains('T') ? Result.Success(day) : Result.Success(day + "T12:00:00Z");
    }

    public Result<string, string> GetDateFromFilename(string filePath)
    {
        var filename = Path.GetFileName(filePath);
        var dateStr = filename?.Substring(0, 10);

        if (DateTime.TryParse(dateStr, out _))
        {
            return Result.Success(dateStr + "T12:00:00Z");
        }

        return Result.Failure<string>($"Date in filename {filename} is not in correct format.");
    }
}

public class MetadataWriter : MetadataManager
{
    public MetadataWriter(ILogger logger) : base(logger) { }

    public Result OverwriteDescription(string filePath, string description)
    {
        var cmd = $"{AtomicParsleyPath} {filePath} --overWrite --description {description}";
        return RunCommand(cmd);
    }

    public Result RemoveDay(string filePath)
    {
        var cmd = $"{AtomicParsleyPath} {filePath} --overWrite --manualAtomRemove moov.udta.meta.ilst.©day";
        return RunCommand(cmd);
    }

    public Result<string, string> CheckAndModifyDate(string dateStr)
    {
        if (DateTime.TryParse(dateStr, out _))
        {
            return Result.Success(dateStr);
        }

        return Result.Success(dateStr + "T12:00:00Z");
    }

    public Result OverwriteDay(string filePath, string day)
    {
        var removeDayResult = RemoveDay(filePath);
        if (removeDayResult.IsFailure)
        {
            return removeDayResult;
        }

        var cmd = $"{AtomicParsleyPath} {filePath} --overWrite --year {day}";
        return RunCommand(cmd);
    }
}
