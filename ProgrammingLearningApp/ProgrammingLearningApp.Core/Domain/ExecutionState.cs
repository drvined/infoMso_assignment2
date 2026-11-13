namespace ProgrammingLearningApp.Domain;

//everything command needs while a program runs
// character, trace of executed commands, path  and optionally the grid
public sealed class ExecutionState
{
    public const int DefaultMaxSteps = 10_000;

    private readonly List<string> _trace = [];
    private readonly List<Position> _path = [];
    private readonly Grid? _grid;
    private readonly int _maxSteps;
    private int _stepCount;

    public Character Character { get; }

    public Grid? Grid => _grid;

    public IReadOnlyList<string> Trace => _trace;

    public IReadOnlyList<Position> Path => _path;

    public ExecutionState(Character character) : this(character, null)
    {
    }

    public ExecutionState(Character character, Grid? grid, int maxSteps = DefaultMaxSteps)
    {
        ArgumentNullException.ThrowIfNull(character);

        Character = character;
        _grid = grid;
        _maxSteps = maxSteps;
        _path.Add(character.Position);
    }

    //counts one unit of work
    public void CountStep()
    {
        _stepCount++;
        if (_stepCount > _maxSteps)
        {
            throw new ExecutionLimitException(
                $"The program took more than {_maxSteps} steps, it probably runs forever.");
        }
    }

    //moves the character one cel
    public void StepForward()
    {
        CountStep();

        if (_grid is not null)
        {
            var target = Character.Position.Step(Character.Direction);

            if (!_grid.Contains(target))
            {
                throw new OutOfGridException($"Cannot move to ({target.X}, {target.Y}): outside the grid.");
            }

            if (_grid.IsBlocked(target))
            {
                throw new WallCollisionException($"Cannot move to ({target.X}, {target.Y}): a wall is in the way.");
            }
        }

        Character.StepForward();
        _path.Add(Character.Position);
    }

    public void AddToTrace(string entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        _trace.Add(entry);
    }
}