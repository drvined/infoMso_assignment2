using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class RepeatUntilTests
{
    [Fact]
    public void RepeatUntilWallAhead_StopsJustBeforeTheWall()
    {
        //3 wide and 2 high, the cell at (2,0) is blocked
        var cells = new CellType[3, 2];
        cells[2, 0] = CellType.Blocked;
        var grid = new Grid(cells);

        var character = new Character(0, 0, Direction.East);
        var state = new ExecutionState(character, grid);
        var loop = new RepeatUntil(new WallAheadCondition(), new List<Command> { new Move(1) });

        loop.Execute(state);

        Assert.Equal(new Position(1, 0), character.Position);
    }

    [Fact]
    public void RepeatUntil_WithConditionThatNeverBecomesTrue_IsStoppedByStepLimit()
    {
        //no grid, so WallAhead is always false and the loop would run forever
        var state = new ExecutionState(new Character(0, 0, Direction.East), null, maxSteps: 5);
        var loop = new RepeatUntil(new WallAheadCondition(), new List<Command> { new Turn(TurnSide.Left) });

        Assert.Throws<ExecutionLimitException>(() => loop.Execute(state));
    }
}