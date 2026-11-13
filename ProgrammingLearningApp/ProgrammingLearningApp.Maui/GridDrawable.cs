using Microsoft.Maui.Graphics;
using ProgrammingLearningApp.Domain;
using DomainGrid = ProgrammingLearningApp.Domain.Grid;

namespace ProgrammingLearningApp.Maui;

//draws the grid, the path and the character on its last cell
//without grid only the path is drawn
public sealed class GridDrawable : IDrawable
{
    //maxz size cell without grid
    private const float MaxFreeCellSize = 40;

    private static readonly Color EmptyColor = Color.FromArgb("#2A2A30");
    private static readonly Color WallColor = Color.FromArgb("#5A5A66");
    private static readonly Color ExitColor = Color.FromArgb("#3DDC84");
    private static readonly Color StartColor = Color.FromArgb("#FFC857");
    private static readonly Color LineColor = Color.FromArgb("#3A3A42");
    private static readonly Color PathColor = Color.FromArgb("#7C5CFF");
    private static readonly Color CharacterColor = Color.FromArgb("#FF6FB5");
    private static readonly Color HintColor = Color.FromArgb("#8B8B96");

    private DomainGrid? _grid;
    private IReadOnlyList<Position> _path = [];
    private Direction? _facing;

    public void Update(DomainGrid? grid, IReadOnlyList<Position> path, Direction? facing)
    {
        _grid = grid;
        _path = path;
        _facing = facing;
    }

    public void Clear() => Update(null, [], null);

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        if (_grid is null && _path.Count == 0)
        {
            DrawHint(canvas, dirtyRect);
            return;
        }

        var area = VisibleArea();
        var cellSize = Math.Min(dirtyRect.Width / area.Width, dirtyRect.Height / area.Height);
        if (_grid is null)
        {
            cellSize = Math.Min(cellSize, MaxFreeCellSize);
        }

        var view = new CellLayout(
            cellSize,
            dirtyRect.Left + (dirtyRect.Width - cellSize * area.Width) / 2 - area.Left * cellSize,
            dirtyRect.Top + (dirtyRect.Height - cellSize * area.Height) / 2 - area.Top * cellSize);

        DrawCells(canvas, area, view);
        DrawPath(canvas, view);
        DrawCharacter(canvas, view);
    }

    //the cells that have to fit in the view
    private Area VisibleArea()
    {
        if (_grid is not null)
        {
            return new Area(0, 0, _grid.Width, _grid.Height);
        }

        var minX = _path.Min(p => p.X);
        var minY = _path.Min(p => p.Y);
        var maxX = _path.Max(p => p.X);
        var maxY = _path.Max(p => p.Y);
        return new Area(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private void DrawCells(ICanvas canvas, Area area, CellLayout view)
    {
        canvas.StrokeColor = LineColor;
        canvas.StrokeSize = 1;

        for (var y = area.Top; y < area.Top + area.Height; y++)
        {
            for (var x = area.Left; x < area.Left + area.Width; x++)
            {
                var position = new Position(x, y);
                var left = view.OffsetX + x * view.CellSize;
                var top = view.OffsetY + y * view.CellSize;

                canvas.FillColor = _grid is null ? EmptyColor : ColorFor(_grid.GetCell(position));
                canvas.FillRectangle(left, top, view.CellSize, view.CellSize);
                canvas.DrawRectangle(left, top, view.CellSize, view.CellSize);
            }
        }

        if (_grid is null)
        {
            var start = view.Center(_path[0]);
            canvas.FillColor = StartColor;
            canvas.FillCircle(start.X, start.Y, view.CellSize * 0.15f);
        }
    }

    private void DrawPath(ICanvas canvas, CellLayout view)
    {
        if (_path.Count < 2)
        {
            return;
        }

        canvas.StrokeColor = PathColor;
        canvas.StrokeSize = Math.Max(2, view.CellSize / 6);
        canvas.StrokeLineCap = LineCap.Round;

        for (var i = 1; i < _path.Count; i++)
        {
            var from = view.Center(_path[i - 1]);
            var to = view.Center(_path[i]);
            canvas.DrawLine(from.X, from.Y, to.X, to.Y);
        }
    }

    //the character on the cell where it ends the triangle pointing the way it faces
    private void DrawCharacter(ICanvas canvas, CellLayout view)
    {
        if (_path.Count == 0)
        {
            return;
        }

        var center = view.Center(_path[^1]);
        var size = view.CellSize * 0.35f;
        canvas.FillColor = CharacterColor;

        if (_facing is not { } facing)
        {
            canvas.FillCircle(center.X, center.Y, size);
            return;
        }

        var (dx, dy) = facing switch
        {
            Direction.North => (0f, -1f),
            Direction.East => (1f, 0f),
            Direction.South => (0f, 1f),
            _ => (-1f, 0f)
        };

        var triangle = new PathF();
        triangle.MoveTo(center.X + dx * size, center.Y + dy * size);
        triangle.LineTo(center.X - dx * size * 0.7f - dy * size * 0.8f, center.Y - dy * size * 0.7f + dx * size * 0.8f);
        triangle.LineTo(center.X - dx * size * 0.7f + dy * size * 0.8f, center.Y - dy * size * 0.7f - dx * size * 0.8f);
        triangle.Close();
        canvas.FillPath(triangle);
    }

    private static void DrawHint(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FontColor = HintColor;
        canvas.FontSize = 14;
        canvas.DrawString("Voer een programma uit of laad een oefening om Juba te laten lopen.", dirtyRect,
            Microsoft.Maui.Graphics.HorizontalAlignment.Center,
            Microsoft.Maui.Graphics.VerticalAlignment.Center);
    }

    private static Color ColorFor(CellType type) => type switch
    {
        CellType.Blocked => WallColor,
        CellType.Exit => ExitColor,
        CellType.Start => StartColor,
        _ => EmptyColor
    };

    //block of cells ( in cell coords)
    private readonly record struct Area(int Left, int Top, int Width, int Height);

    //where the drawing is on the screen
    private readonly record struct CellLayout(float CellSize, float OffsetX, float OffsetY)
    {
        public PointF Center(Position position) =>
            new(OffsetX + (position.X + 0.5f) * CellSize, OffsetY + (position.Y + 0.5f) * CellSize);
    }
}
