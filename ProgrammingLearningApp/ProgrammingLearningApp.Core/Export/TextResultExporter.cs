using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Export;

//exports the result of a run (trace and end state) as text and can save it as a .txt file
//not to be confused with TextProgramExporter, which exports the program itself
public sealed class TextResultExporter
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