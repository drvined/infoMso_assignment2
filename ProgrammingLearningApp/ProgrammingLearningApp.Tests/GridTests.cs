using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class GridTests
{
    //3 wide and 2 high, only the cell at (1,0) is blocked
    private static Grid CreateGrid()
    {
        var cells = new CellType[3, 2];
        cells[1, 0] = CellType.Blocked;
        return new Grid(cells);
    }

    [Fact]
    public void GetCell_ReturnsTheCellAtThePosition()
    {
        var grid = CreateGrid();

        Assert.Equal(CellType.Blocked, grid.GetCell(new Position(1, 0)));
        Assert.Equal(CellType.Open, grid.GetCell(new Position(0, 1)));
    }

    [Fact]
    public void GetCell_OutsideTheGrid_Throws()
    {
        var grid = CreateGrid();

        Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetCell(new Position(3, 0)));
    }

    [Fact]
    public void IsBlocked_TrueForBlockedCell()
    {
        Assert.True(CreateGrid().IsBlocked(new Position(1, 0)));
    }

    [Fact]
    public void IsBlocked_FalseForOpenCell()
    {
        Assert.False(CreateGrid().IsBlocked(new Position(0, 1)));
    }

    [Fact]
    public void IsBlocked_FalseOutsideTheGrid()
    {
        Assert.False(CreateGrid().IsBlocked(new Position(3, 0)));
    }
}