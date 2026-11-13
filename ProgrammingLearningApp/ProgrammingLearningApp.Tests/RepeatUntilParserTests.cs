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
    public void Parse_RepeatUntilAsOneWord_IsAlsoARepeatUntil()
    {
        var program = new ProgramParser().Parse("test", "RepeatUntil GridEdge\n    Move 1");

        var repeatUntil = Assert.IsType<RepeatUntil>(Assert.Single(program.Commands));
        Assert.IsType<GridEdgeCondition>(repeatUntil.Condition);
        Assert.IsType<Move>(Assert.Single(repeatUntil.Commands));
    }

    [Fact]
    public void Parse_FindExit1FromTheAssignment_IsParsed()
    {
        const string findExit1 =
            "Repeat 3\n    RepeatUntil WallAhead\n        Move 1\n    Turn right\n" +
            "Repeat 2\n    RepeatUntil GridEdge\n        Move 1\n    Turn right";

        var program = new ProgramParser().Parse("FindExit1", findExit1);

        Assert.Equal(2, program.Commands.Count);
        Assert.All(program.Commands, command => Assert.IsType<Repeat>(command));
    }

    [Theory]
    [InlineData("WallAhead")]
    [InlineData("GridEdge")]
    public void ParsedCondition_HasTheNameItWasWrittenWith(string conditionName)
    {
        var program = new ProgramParser().Parse("test", $"RepeatUntil {conditionName}\n    Move 1");

        var repeatUntil = Assert.IsType<RepeatUntil>(Assert.Single(program.Commands));
        Assert.Equal(conditionName, repeatUntil.Condition.Name);
    }

    [Fact]
    public void Parse_UnknownCondition_ThrowsFormatException()
    {
        Assert.Throws<FormatException>(() =>
            new ProgramParser().Parse("test", "Repeat until Lava\n    Move 1"));
    }
}