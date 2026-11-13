using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//what you get after running a program: the trace of all commands that were done,
//the path the character walked (for drawing) and the end state of the character
public sealed class ExecutionResult
{
    private readonly List<string> _trace;
    private readonly List<Position> _path;

    public IReadOnlyList<string> Trace => _trace;

    //every cell the character was on, in order, starting with the start cell
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
