using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;
using ProgrammingLearningApp.Parsing;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class PartialPathTests
{
    [Fact]
    public void RuntimeError_KeepsThePathWalkedSoFar()
    {
        //5 wide, the cell at (4,0) is a wall, so Move 10 stops at (3,0)
        var grid = new GridParser().Parse("sooo+");
        var program = new ProgramParser().Parse("test", "Move 10");

        var error = Assert.Throws<WallCollisionException>(() =>
            new ProgramExecutor().Execute(program, grid, new Character(0, 0, Direction.East)));

        Assert.Equal(4, error.Path.Count);
        Assert.Equal(new Position(3, 0), error.Path[^1]);
    }
}
