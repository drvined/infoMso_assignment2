namespace ProgrammingLearningApp.Domain;

//one cell on the grid. X goes east, Y goes south, so (0,0) is the top-left cell
public readonly record struct Position(int X, int Y)
{
    //the cell next to this one in the given direction
    public Position Step(Direction direction) => direction switch
    {
        Direction.North => this with { Y = Y - 1 },
        Direction.South => this with { Y = Y + 1 },
        Direction.East => this with { X = X + 1 },
        Direction.West => this with { X = X - 1 },
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown direction.")
    };
}