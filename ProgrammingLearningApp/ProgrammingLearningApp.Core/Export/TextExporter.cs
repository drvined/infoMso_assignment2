using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Export;

//turns execution result into text and can save as  .txt file
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