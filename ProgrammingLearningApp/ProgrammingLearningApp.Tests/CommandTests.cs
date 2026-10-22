using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class CommandTests
{
    [Fact]
    public void Move_NegativeSteps_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Move(-1));
    }

    [Fact]
    public void Repeat_NegativeTimes_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Repeat(-1, [new Move(1)]));
    }

    [Fact]
    public void Repeat_KeepsItsCommandsInOrder()
    {
        var move = new Move(2);
        var turn = new Turn(TurnSide.Right);

        var repeat = new Repeat(3, [move, turn]);

        Assert.Equal(3, repeat.Times);
        Assert.Equal(new Command[] { move, turn }, repeat.Commands);
    }
}
