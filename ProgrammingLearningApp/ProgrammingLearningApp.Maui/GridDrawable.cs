using Microsoft.Maui.Graphics;
using ProgrammingLearningApp.Domain;
using DomainGrid = ProgrammingLearningApp.Domain.Grid;

namespace ProgrammingLearningApp.Maui;

//draws the grid and the path the character walked over it
public sealed class GridDrawable : IDrawable
{
    private DomainGrid? _grid;
    private IReadOnlyList<Position> _path = [];

    public void Update(DomainGrid grid, IReadOnlyList<Position> path)
    {
        _grid = grid;
        _path = path;
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_grid is null)
        {
            return;
        }

        var cellSize = Math.Min(dirtyRect.Width / _grid.Width, dirtyRect.Height / _grid.Height);
        var offsetX = dirtyRect.Left;
        var offsetY = dirtyRect.Top;

        for (var y = 0; y < _grid.Height; y++)
        {
            for (var x = 0; x < _grid.Width; x++)
            {
                var left = offsetX + x * cellSize;
                var top = offsetY + y * cellSize;

                canvas.FillColor = ColorFor(_grid.GetCell(new Position(x, y)));
                canvas.FillRectangle(left, top, cellSize, cellSize);
                canvas.StrokeColor = Colors.Gray;
                canvas.DrawRectangle(left, top, cellSize, cellSize);
            }
        }

        if (_path.Count < 2)
        {
            return;
        }

        canvas.StrokeColor = Colors.Blue;
        canvas.StrokeSize = 3;

        for (var i = 1; i < _path.Count; i++)
        {
            var from = Center(_path[i - 1], cellSize, offsetX, offsetY);
            var to = Center(_path[i], cellSize, offsetX, offsetY);
            canvas.DrawLine(from.X, from.Y, to.X, to.Y);
        }
    }

    private static PointF Center(Position position, float cellSize, float offsetX, float offsetY) =>
        new(offsetX + (position.X + 0.5f) * cellSize, offsetY + (position.Y + 0.5f) * cellSize);

    private static Color ColorFor(CellType type) => type switch
    {
        CellType.Blocked => Colors.DarkGray,
        CellType.Exit => Colors.LightGreen,
        CellType.Start => Colors.Yellow,
        _ => Colors.White
    };
}