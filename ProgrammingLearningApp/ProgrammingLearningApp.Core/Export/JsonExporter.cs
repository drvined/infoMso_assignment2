using System.Text.Json;
using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Export;

//turns execution result into json and can save .json file
public sealed class JsonExporter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public string ToJson(ExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        var export = new JsonExport(
            result.Trace,
            new JsonEndState(result.FinalX, result.FinalY, result.FinalDirection.ToString().ToLowerInvariant()),
            result.Path.Select(p => new JsonPosition(p.X, p.Y)).ToList());

        return JsonSerializer.Serialize(export, Options);
    }

    public void Save(ExecutionResult result, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        File.WriteAllText(path, ToJson(result));
    }

    public sealed record JsonExport(IReadOnlyList<string> Trace, JsonEndState EndState, List<JsonPosition> Path);

    public sealed record JsonEndState(int X, int Y, string Direction);

    public sealed record JsonPosition(int X, int Y);
}