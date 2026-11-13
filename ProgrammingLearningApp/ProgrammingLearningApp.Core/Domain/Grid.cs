namespace ProgrammingLearningApp.Domain;

public enum CellType
{
    Open,
    Blocked,
    Exit,
    Start
}

public sealed class Grid
{
    private readonly CellType[,] _cells;

    public Grid(CellType[,] cells)
    {
        ArgumentNullException.ThrowIfNull(cells);
        _cells = cells;
    }

    public int Width => _cells.GetLength(0);

    public int Height => _cells.GetLength(1);

    public bool Contains(Position position) =>
        position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height;
}