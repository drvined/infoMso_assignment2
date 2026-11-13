using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//everything a command needs while a program runs: the character, the trace of
//executed commands, the path (every cell the character has been on) and optionally the grid
//(not called ExecutionContext because that name already exists in System.Threading)
public sealed class ExecutionState
{
    private readonly List<string> _trace = [];
    private readonly List<Position> _path = [];
    private readonly Grid? _grid;

    public Character Character { get; }

    public IReadOnlyList<string> Trace => _trace;

    //starts with the start cell, then one entry per step
    public IReadOnlyList<Position> Path => _path;

    //without a grid there are no wall or edge checks
    public ExecutionState(Character character) : this(character, null)
    {
    }

    public ExecutionState(Character character, Grid? grid)
    {
        ArgumentNullException.ThrowIfNull(character);

        Character = character;
        _grid = grid;
        _path.Add(character.Position);
    }

    //moves the character one cell and remembers it in the path
    //every step goes through here, so the grid checks happen here
    public void StepForward()
    {
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