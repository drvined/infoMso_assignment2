using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ProgramParserTests
{
    [Fact]
    public void Example1_HasEightCommands()
    {
        GridProgram program = TestPrograms.Parse(TestPrograms.Example1);

        Assert.Equal(8, program.Commands.Count);
    }

    [Fact]
    public void Example2_IsOneRepeatWithTwoCommands()
    {
        GridProgram program = TestPrograms.Parse(TestPrograms.Example2);

        Repeat repeat = Assert.IsType<Repeat>(Assert.Single(program.Commands));
        Assert.Equal(4, repeat.Times);
        Assert.Equal(2, repeat.Commands.Count);
    }

    [Fact]
    public void MoveAndTurn_KeepTheirValues()
    {
        GridProgram program = TestPrograms.Parse("Move 3\nTurn left");

        Move move = Assert.IsType<Move>(program.Commands[0]);
        Turn turn = Assert.IsType<Turn>(program.Commands[1]);
        Assert.Equal(3, move.Steps);
        Assert.Equal(TurnSide.Left, turn.Side);
    }

    [Fact]
    public void Parse_UsesTheGivenName()
    {
        GridProgram program = new ProgramParser().Parse("Rectangle1", TestPrograms.Example1);

        Assert.Equal("Rectangle1", program.Name);
    }

    [Fact]
    public void UnknownCommand_Throws()
    {
        Assert.Throws<FormatException>(() => TestPrograms.Parse("Jump 3"));
    }

    [Fact]
    public void IndentationNotAMultipleOfFour_Throws()
    {
        Assert.Throws<FormatException>(() => TestPrograms.Parse("Repeat 2 times\n  Move 1"));
    }

    [Fact]
    public void UnexpectedIndentation_Throws()
    {
        Assert.Throws<FormatException>(() => TestPrograms.Parse("Move 1\n        Move 2"));
    }

    [Fact]
    public void RepeatWithoutBody_Throws()
    {
        Assert.Throws<FormatException>(() => TestPrograms.Parse("Repeat 2 times"));
    }

    [Theory]
    [InlineData("Move 1\rTurn right\rMove 2")]
    [InlineData("Move 1\r\nTurn right\r\nMove 2")]
    [InlineData("Move 1\nTurn right\nMove 2")]
    public void Parse_WorksWithEveryKindOfLineEnding(string text)
    {
        GridProgram program = TestPrograms.Parse(text);

        Assert.Equal(3, program.Commands.Count);
    }

    [Fact]
    public void RepeatWithoutTimes_IsAlsoARepeat()
    {
        GridProgram program = TestPrograms.Parse("Repeat 3\n    Move 1");

        Repeat repeat = Assert.IsType<Repeat>(Assert.Single(program.Commands));
        Assert.Equal(3, repeat.Times);
    }

    [Fact]
    public void TabIndentation_Throws()
    {
        Assert.Throws<FormatException>(() => TestPrograms.Parse("Repeat 2 times\n\tMove 1"));
    }
}