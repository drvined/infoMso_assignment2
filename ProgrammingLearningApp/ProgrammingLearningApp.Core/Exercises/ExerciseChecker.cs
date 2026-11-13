using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Parsing;

namespace ProgrammingLearningApp.Exercises;

//checks if a program brings the character from the start cell to the exit cell
public sealed class ExerciseChecker
{
    private readonly GridParser _gridParser = new();
    private readonly IProgramRunner _runner;

    //the runner is a parameter so tests can pass a fake; by default the real executor is used
    public ExerciseChecker(IProgramRunner? runner = null)
    {
        _runner = runner ?? new ProgramExecutor();
    }

    public bool IsSolved(GridProgram program, PathfindingExercise exercise)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(exercise);

        var grid = _gridParser.Parse(exercise.GridText);
        var start = FindCell(grid, CellType.Start);
        var exit = FindCell(grid, CellType.Exit);

        try
        {
            var result = _runner.Execute(program, grid, new Character(start.X, start.Y, Direction.East));
            return result.FinalX == exit.X && result.FinalY == exit.Y;
        }
        catch (ProgramRuntimeException)
        {
            //a wall or the edge of the grid stops the program, so the exercise is not solved
            return false;
        }
    }

    private static Position FindCell(Grid grid, CellType type)
    {
        for (var y = 0; y < grid.Height; y++)
        {
            for (var x = 0; x < grid.Width; x++)
            {
                if (grid.GetCell(new Position(x, y)) == type)
                {
                    return new Position(x, y);
                }
            }
        }

        throw new InvalidOperationException($"The grid has no {type} cell.");
    }
}
