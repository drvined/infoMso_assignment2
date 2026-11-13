using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Parsing;

// Reads a grid from text. Each line is a row and each character is a cell.
public sealed class GridParser
{
    public Grid Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var rows = text
            .Split('\n')
            .Select(row => row.TrimEnd('\r'))
            .Where(row => row.Length > 0)
            .ToList();

        if (rows.Count == 0)
        {
            throw new FormatException("The grid is empty.");
        }

        var width = rows[0].Length;
        var cells = new CellType[width, rows.Count];

        for (var y = 0; y < rows.Count; y++)
        {
            if (rows[y].Length != width)
            {
                throw new FormatException($"Row {y + 1} has {rows[y].Length} cells, but row 1 has {width}.");
            }

            for (var x = 0; x < width; x++)
            {
                cells[x, y] = ToCell(rows[y][x]);
            }
        }

        return new Grid(cells);
    }

    private static CellType ToCell(char symbol) => symbol switch
    {
        'o' => CellType.Open,
        '+' => CellType.Blocked,
        'x' => CellType.Exit,
        's' => CellType.Start,
        _ => throw new FormatException($"Unknown cell '{symbol}'. Use o, +, x or s.")
    };
}