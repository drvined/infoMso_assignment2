using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Exercises;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ExerciseCheckerTests
{
    private readonly ExerciseChecker _checker = new();
    private readonly ProgramParser _parser = new();

    private static readonly PathfindingExercise Straight = new("Recht vooruit", "", "soox");
    private static readonly PathfindingExercise AroundWall = new("Om de muur heen", "", "soo\no+o\noox");
    private static readonly PathfindingExercise Zigzag = new("Zigzag", "", "s+o\noo+\n+ox");

    [Fact]
    public void StraightExercise_SolvedByMoveThree()
    {
        Assert.True(_checker.IsSolved(_parser.Parse("test", "Move 3"), Straight));
    }

    [Fact]
    public void StraightExercise_NotSolvedByMoveTwo()
    {
        Assert.False(_checker.IsSolved(_parser.Parse("test", "Move 2"), Straight));
    }

    [Fact]
    public void AroundWallExercise_SolvedByGoingAroundTheWall()
    {
        Assert.True(_checker.IsSolved(_parser.Parse("test", "Move 2\nTurn right\nMove 2"), AroundWall));
    }

    [Fact]
    public void AroundWallExercise_NotSolvedByWalkingIntoTheWall()
    {
        Assert.False(_checker.IsSolved(_parser.Parse("test", "Move 1\nTurn right\nMove 2"), AroundWall));
    }

    [Fact]
    public void ZigzagExercise_SolvedByZigzagging()
    {
        var program = _parser.Parse("test",
            "Turn right\nMove 1\nTurn left\nMove 1\nTurn right\nMove 1\nTurn left\nMove 1");

        Assert.True(_checker.IsSolved(program, Zigzag));
    }

    //with a fake runner the checker is tested on its own, without running a real program
    [Fact]
    public void IsSolved_TrueWhenTheRunnerEndsOnTheExit()
    {
        var runner = new FakeProgramRunner(new ExecutionResult([], [new Position(0, 0)], 3, 0, Direction.East));
        var checker = new ExerciseChecker(runner);

        Assert.True(checker.IsSolved(new GridProgram("fake", []), Straight));
    }

    [Fact]
    public void IsSolved_PassesTheStartAndTheGridOfTheExercise()
    {
        var runner = new FakeProgramRunner(new ExecutionResult([], [new Position(0, 0)], 3, 0, Direction.East));
        var checker = new ExerciseChecker(runner);

        checker.IsSolved(new GridProgram("fake", []), Straight);

        Assert.Equal(new Position(0, 0), runner.LastStart);
        Assert.Equal(4, runner.LastGrid!.Width);
    }

    [Fact]
    public void IsSolved_FalseWhenTheRunnerEndsElsewhere()
    {
        var runner = new FakeProgramRunner(new ExecutionResult([], [new Position(0, 0)], 2, 0, Direction.East));
        var checker = new ExerciseChecker(runner);

        Assert.False(checker.IsSolved(new GridProgram("fake", []), Straight));
    }

    //the grid format of the assignment has no s: the character then starts top-left
    [Fact]
    public void ExerciseWithoutStartCell_StartsTopLeft()
    {
        var withoutStart = new PathfindingExercise("Zonder s", "", "oo+\n+ox");

        Assert.True(_checker.IsSolved(_parser.Parse("test", "Move 1\nTurn right\nMove 1\nTurn left\nMove 1"),
            withoutStart));
    }

    [Fact]
    public void ReachedExit_TrueOnlyWhenTheRunEndsOnTheX()
    {
        var grid = new GridParser().Parse("soox");
        var onExit = new ExecutionResult([], [new Position(3, 0)], 3, 0, Direction.East);
        var beforeExit = new ExecutionResult([], [new Position(2, 0)], 2, 0, Direction.East);

        Assert.True(_checker.ReachedExit(grid, onExit));
        Assert.False(_checker.ReachedExit(grid, beforeExit));
    }

    [Fact]
    public void ReachedExit_FalseWhenTheGridHasNoExit()
    {
        var grid = new GridParser().Parse("sooo");
        var result = new ExecutionResult([], [new Position(3, 0)], 3, 0, Direction.East);

        Assert.False(_checker.ReachedExit(grid, result));
    }

    [Fact]
    public void IsSolved_FalseWhenTheRunnerReportsARuntimeError()
    {
        var runner = new FakeProgramRunner(new WallCollisionException("a wall is in the way"));
        var checker = new ExerciseChecker(runner);

        Assert.False(checker.IsSolved(new GridProgram("fake", []), Straight));
    }
}
