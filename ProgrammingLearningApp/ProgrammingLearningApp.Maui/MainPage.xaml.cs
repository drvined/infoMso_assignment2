using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Parsing;
using DomainGrid = ProgrammingLearningApp.Domain.Grid;

namespace ProgrammingLearningApp.Maui;

public partial class MainPage : ContentPage
{
    private readonly ProgramParser _programParser = new();
    private readonly GridParser _gridParser = new();
    private readonly ProgramExecutor _executor = new();

    public MainPage()
    {
        InitializeComponent();
    }

    private void OnRunClicked(object? sender, EventArgs e)
    {
        try
        {
            var program = _programParser.Parse("program", ProgramEditor.Text ?? string.Empty);
            var grid = _gridParser.Parse(GridEditor.Text ?? string.Empty);

            var start = FindStart(grid);
            var character = new Character(start.X, start.Y, Direction.East);

            var result = _executor.Execute(program, grid, character);
            ResultLabel.Text = result.ToString();
        }
        catch (FormatException ex)
        {
            ResultLabel.Text = ex.Message;
        }
        catch (ProgramRuntimeException ex)
        {
            ResultLabel.Text = ex.Message;
        }
    }

    //the start cell (s) in the grid, or (0,0) when the grid has none
    private static Position FindStart(DomainGrid grid)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                if (grid.GetCell(new Position(x, y)) == CellType.Start)
                {
                    return new Position(x, y);
                }
            }
        }

        return new Position(0, 0);
    }
}