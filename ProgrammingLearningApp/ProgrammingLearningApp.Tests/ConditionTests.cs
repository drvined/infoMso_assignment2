using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ConditionTests
{
    //3 wide and 2 high, only the cell at (1,0) is blocked
    private static Grid CreateGrid()
    {
        var cells = new CellType[3, 2];
        cells[1, 0] = CellType.Blocked;
        return new Grid(cells);
    }

    [Fact]
    public void WallAhead_TrueWhenWallIsDirectlyInFront()
    {
        var state = new ExecutionState(new Character(0, 0, Direction.East), CreateGrid());

        Assert.True(new WallAheadCondition().IsTrue(state));
    }

    [Fact]
    public void WallAhead_FalseWhenCellInFrontIsOpen()
    {
        var state = new ExecutionState(new Character(0, 0, Direction.South), CreateGrid());

        Assert.False(new WallAheadCondition().IsTrue(state));
    }

    [Fact]
    public void WallAhead_FalseWithoutGrid()
    {
        var state = new ExecutionState(new Character(0, 0, Direction.East));

        Assert.False(new WallAheadCondition().IsTrue(state));
    }

    [Fact]
    public void GridEdge_TrueWhenCellInFrontIsOutsideTheGrid()
    {
        var state = new ExecutionState(new Character(2, 0, Direction.East), CreateGrid());

        Assert.True(new GridEdgeCondition().IsTrue(state));
    }

    [Fact]
    public void GridEdge_FalseWhenCellInFrontIsInsideTheGrid()
    {
        var state = new ExecutionState(new Character(0, 0, Direction.South), CreateGrid());

        Assert.False(new GridEdgeCondition().IsTrue(state));
    }

    [Fact]
    public void GridEdge_FalseWithoutGrid()
    {
        var state = new ExecutionState(new Character(0, 0, Direction.North));

        Assert.False(new GridEdgeCondition().IsTrue(state));
    }
}