using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class GridParserTests
{
    [Fact]
    public void Parse_GivesCorrectWidthAndHeight()
    {
        var grid = new GridParser().Parse("ooo+\noooo");

        Assert.Equal(4, grid.Width);
        Assert.Equal(2, grid.Height);
    }

    [Fact]
    public void Contains_IsTrueInsideTheGrid_AndFalseOutside()
    {
        var grid = new GridParser().Parse("ooo+\noooo");

        Assert.True(grid.Contains(new Position(0, 0)));
        Assert.True(grid.Contains(new Position(3, 1)));
        Assert.False(grid.Contains(new Position(4, 0)));
        Assert.False(grid.Contains(new Position(0, 2)));
        Assert.False(grid.Contains(new Position(-1, 0)));
    }
    [Fact]
    public void Parse_UnknownCharacter_Throws()
    {
        Assert.Throws<FormatException>(() => new GridParser().Parse("oz"));
    }
    [Theory]
    [InlineData("ooo+\roooo")]
    [InlineData("ooo+\r\noooo")]
    [InlineData("ooo+\noooo\n")]
    public void Parse_WorksWithEveryKindOfLineEnding(string text)
    {
        var grid = new GridParser().Parse(text);

        Assert.Equal(4, grid.Width);
        Assert.Equal(2, grid.Height);
    }

    [Fact]
    public void Parse_RowsOfDifferentLength_Throws()
    {
        Assert.Throws<FormatException>(() => new GridParser().Parse("ooo\noo"));
    }
}