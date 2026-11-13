namespace ProgrammingLearningApp.Domain;

//everything a command needs while a program runs: the character, the trace of
//executed commands, the path (every cell the character has been on) and optionally the grid
//(not called ExecutionContext because that name already exists in System.Threading)
public sealed class ExecutionState
{
    public const int DefaultMaxSteps = 10_000;

    private readonly List<string> _trace = [];
    private readonly List<Position> _path = [];
    private readonly Grid? _grid;
    private readonly int _maxSteps;
    private int _stepCount;

    public Character Character { get; }

    //null when the program runs without a grid
    public Grid? Grid => _grid;

    public IReadOnlyList<string> Trace => _trace;

    //starts with the start cell, then one entry per step
    public IReadOnlyList<Position> Path => _path;

    //without a grid there are no wall or edge checks
    public ExecutionState(Character character) : this(character, null)
    {
    }

    //maxSteps stops the program when it runs too long, probably an endless loop
    public ExecutionState(Character character, Grid? grid, int maxSteps = DefaultMaxSteps)
    {
        ArgumentNullException.ThrowIfNull(character);

        Character = character;
        _grid = grid;
        _maxSteps = maxSteps;
        _path.Add(character.Position);
    }

    //counts one unit of work; every command that can loop calls this,
    //so an endless loop is stopped even when it never moves the character
    public void CountStep()
    {
        _stepCount++;
        if (_stepCount > _maxSteps)
        {
            throw new ExecutionLimitException(
                $"The program took more than {_maxSteps} steps, it probably runs forever.");
        }
    }

    //moves the character one cell and remembers it in the path
    //every step goes through here, so the step limit and the grid checks happen here
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