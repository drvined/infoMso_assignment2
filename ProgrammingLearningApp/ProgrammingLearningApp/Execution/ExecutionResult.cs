using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

public sealed class ExecutionResult
{
    private readonly List<string> _trace;

    public IReadOnlyList<string> Trace => _trace;

    public int FinalX { get; }

    public int FinalY { get; }

    public Direction FinalDirection { get; }

    public ExecutionResult(IEnumerable<string> trace, int finalX, int finalY, Direction finalDirection)
    {
        ArgumentNullException.ThrowIfNull(trace);

        _trace = trace.ToList();
        FinalX = finalX;
        FinalY = finalY;
        FinalDirection = finalDirection;
    }

    //for example: move 10, turn right, move 10
    public string FormatTrace() =>
        _trace.Count == 0 ? string.Empty : string.Join(", ", _trace) + ".";

    //end state (0,0) facing east
    public string FormatEndState() =>
        $"End state ({FinalX},{FinalY}) facing {FinalDirection.ToString().ToLowerInvariant()}.";

    public override string ToString()
    {
        var trace = FormatTrace();
        var endState = FormatEndState();
        return trace.Length == 0 ? endState : trace + Environment.NewLine + endState;
    }
}
