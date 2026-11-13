using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Export;

//turns an execution result into plain text and can save that text as a .txt file
public sealed class TextExporter
{
    public string ToText(ExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        return $"{result.FormatTrace()}{Environment.NewLine}{result.FormatEndState()}{Environment.NewLine}";
    }

    public void Save(ExecutionResult result, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        File.WriteAllText(path, ToText(result));
    }
}