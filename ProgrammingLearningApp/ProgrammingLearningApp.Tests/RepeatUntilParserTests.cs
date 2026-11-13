using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class RepeatUntilParserTests
{
    [Fact]
    public void Parse_RepeatUntilWallAhead_RunsUntilTheWall()
    {
        //3 wide and 2 high, the cell at (2,0) is blocked
        var cells = new CellType[3, 2];
        cells[2, 0] = CellType.Blocked;
        var grid = new Grid(cells);

        var program = new ProgramParser().Parse("test", "Repeat until WallAhead\n    Move 1");
        var character = new Character(0, 0, Direction.East);

        var result = new ProgramExecutor().Execute(program, grid, character);

        Assert.Equal(1, result.FinalX);
        Assert.Equal(0, result.FinalY);
    }

    [Fact]
    public void Parse_UnknownCondition_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            new ProgramParser().Parse("test", "Repeat until Lava\n    Move 1"));
    }
}