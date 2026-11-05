using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class GridParserTests
{
    //Checks if the grid gets the right width and highth
    [Fact]
    public void Parse_GivesCorrectWidthAndHeight()
    {
        var grid = new GridParser().Parse("ooo+\noooo");

        Assert.Equal(4, grid.Width);
        Assert.Equal(2, grid.Height);
    }

    // Checks if Contains is true inside and false outside of the grid
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

    // Checks if a different character gives an error
    [Fact]
    public void Parse_UnknownCharacter_Throws()
    {
        Assert.Throws<FormatException>(() => new GridParser().Parse("oz"));
    }

    //If rows have different lengths it gives an error
    [Fact]
    public void Parse_RowsOfDifferentLength_Throws()
    {
        Assert.Throws<FormatException>(() => new GridParser().Parse("ooo\noo"));
    }
}