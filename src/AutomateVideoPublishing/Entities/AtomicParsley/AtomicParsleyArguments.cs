using System.Text;

namespace AutomateVideoPublishing.Entities.AtomicParsley;

public class AtomicParsleyArguments
{
    private readonly StringBuilder _arguments;

    public AtomicParsleyArguments() => _arguments = new StringBuilder();

    public AtomicParsleyArguments AddFilePath(string filePath)
    {
        _arguments.AppendFormat("\"{0}\" ", filePath);
        return this;
    }

    public AtomicParsleyArguments AddOption(string option)
    {
        _arguments.AppendFormat("{0} ", option);
        return this;
    }

    public AtomicParsleyArguments AddTag(string tagName, string value)
    {
        _arguments.AppendFormat("--{0} \"{1}\" ", tagName, value);
        return this;
    }

    public override string ToString() => _arguments.ToString().Trim();

    public static implicit operator string?(AtomicParsleyArguments arguments) => arguments.ToString();
}
