using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ExecutionStateTests
{
    //3 wide and 2 high, only the cell at (1,0) is blocked
    private static Grid CreateGrid()
    {
        var cells = new CellType[3, 2];
        cells[1, 0] = CellType.Blocked;
        return new Grid(cells);
    }

    [Fact]
    public void StepForward_IntoOpenCell_MovesAndAddsToPath()
    {
        var character = new Character(0, 0, Direction.South);
        var state = new ExecutionState(character, CreateGrid());

        state.StepForward();

        Assert.Equal(new Position(0, 1), character.Position);
        Assert.Equal(2, state.Path.Count);
    }

    [Fact]
    public void StepForward_IntoBlockedCell_ThrowsWallCollision_AndDoesNotMove()
    {
        var character = new Character(0, 0, Direction.East);
        var state = new ExecutionState(character, CreateGrid());

        Assert.Throws<WallCollisionException>(() => state.StepForward());

        Assert.Equal(new Position(0, 0), character.Position);
        Assert.Single(state.Path);
    }

    [Fact]
    public void StepForward_OutsideTheGrid_ThrowsOutOfGrid_AndDoesNotMove()
    {
        var character = new Character(2, 0, Direction.East);
        var state = new ExecutionState(character, CreateGrid());

        Assert.Throws<OutOfGridException>(() => state.StepForward());

        Assert.Equal(new Position(2, 0), character.Position);
    }

    [Fact]
    public void StepForward_WithoutGrid_DoesNotCheckBounds()
    {
        var character = new Character(0, 0, Direction.North);
        var state = new ExecutionState(character);

        state.StepForward();

        Assert.Equal(new Position(0, -1), character.Position);
    }

    [Fact]
    public void StepForward_WhenStepLimitIsReached_ThrowsExecutionLimitException()
    {
        var character = new Character(0, 0, Direction.East);
        var state = new ExecutionState(character, null, maxSteps: 2);

        state.StepForward();
        state.StepForward();

        Assert.Throws<ExecutionLimitException>(() => state.StepForward());
    }
}