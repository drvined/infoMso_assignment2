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

    public CellType GetCell(Position position)
    {
        if (!Contains(position))
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position is outside the grid.");
        }

        return _cells[position.X, position.Y];
    }

    public bool IsBlocked(Position position) =>
        Contains(position) && GetCell(position) == CellType.Blocked;

    //where the character starts: the s cell, or the top-left cell when the grid has no s
    //(the assignment says the character starts top-left, the s is optional)
    public Position StartPosition => Find(CellType.Start) ?? new Position(0, 0);

    //the first cell of this type, reading row by row from the top-left, or null when there is none
    public Position? Find(CellType type)
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (_cells[x, y] == type)
                {
                    return new Position(x, y);
                }
            }
        }

        return null;
    }
}