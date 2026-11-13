using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//the trace of all commands that were done, path the character walked and end state of the character
public sealed class ExecutionResult
{
    private readonly List<string> _trace;
    private readonly List<Position> _path;

    public IReadOnlyList<string> Trace => _trace;

    public IReadOnlyList<Position> Path => _path;

    public int FinalX { get; }

    public int FinalY { get; }

    public Direction FinalDirection { get; }

    public ExecutionResult(IEnumerable<string> trace, IEnumerable<Position> path,
        int finalX, int finalY, Direction finalDirection)
    {
        ArgumentNullException.ThrowIfNull(trace);
        ArgumentNullException.ThrowIfNull(path);

        _trace = trace.ToList();
        _path = path.ToList();
        FinalX = finalX;
        FinalY = finalY;
        FinalDirection = finalDirection;
    }

    public string FormatTrace() =>
        _trace.Count == 0 ? string.Empty : string.Join(", ", _trace) + ".";

    public string FormatEndState() =>
        $"End state ({FinalX},{FinalY}) facing {FinalDirection.ToString().ToLowerInvariant()}.";

    public override string ToString()
    {
        var trace = FormatTrace();
        var endState = FormatEndState();
        return trace.Length == 0 ? endState : trace + Environment.NewLine + endState;
    }
}
