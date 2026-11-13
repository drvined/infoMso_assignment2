using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Exercises;

//checks ig program brings the character from the start cell to the exit cell
public sealed class ExerciseChecker
{
    private readonly IProgramRunner _runner;

    public ExerciseChecker(IProgramRunner? runner = null)
    {
        _runner = runner ?? new ProgramExecutor();
    }

    public bool IsSolved(GridProgram program, PathfindingExercise exercise)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(exercise);

        var grid = exercise.Grid;
        var start = grid.StartPosition;

        try
        {
            var result = _runner.Execute(program, grid, new Character(start.X, start.Y, Direction.East));
            return ReachedExit(grid, result);
        }
        catch (ProgramRuntimeException)
        {
            //wall or edge the grid stops the program
            return false;
        }
    }

    public bool ReachedExit(Grid grid, ExecutionResult result)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(result);

        return grid.Find(CellType.Exit) is { } exit
               && result.FinalX == exit.X
               && result.FinalY == exit.Y;
    }
}
