using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class GridTests
{
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

    [Fact]
    public void Find_ReturnsTheFirstCellOfThatType()
    {
        var found = CreateGrid().Find(CellType.Blocked);

        Assert.NotNull(found);
        Assert.Equal(new Position(1, 0), found.Value);
    }

    [Fact]
    public void Find_NullWhenTheTypeIsNotInTheGrid()
    {
        Assert.Null(CreateGrid().Find(CellType.Exit));
    }

    [Fact]
    public void StartPosition_IsTheStartCell()
    {
        var cells = new CellType[3, 2];
        cells[2, 1] = CellType.Start;

        Assert.Equal(new Position(2, 1), new Grid(cells).StartPosition);
    }

    [Fact]
    public void StartPosition_IsTopLeftWhenThereIsNoStartCell()
    {
        Assert.Equal(new Position(0, 0), CreateGrid().StartPosition);
    }
}