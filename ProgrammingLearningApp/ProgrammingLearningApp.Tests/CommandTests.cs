using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;
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

    [Fact]
    public void Move_Execute_WalksAndAddsToTrace()
    {
        var state = new ExecutionState(new Character());

        new Move(2).Execute(state);

        Assert.Equal(new Position(2, 0), state.Character.Position);
        Assert.Equal(new[] { "Move 2" }, state.Trace);
    }

    [Fact]
    public void Turn_Execute_TurnsAndAddsToTrace()
    {
        var state = new ExecutionState(new Character());

        new Turn(TurnSide.Left).Execute(state);

        Assert.Equal(Direction.North, state.Character.Direction);
        Assert.Equal(new[] { "Turn left" }, state.Trace);
    }

    [Fact]
    public void Repeat_Execute_RunsBodyTheGivenNumberOfTimes()
    {
        var state = new ExecutionState(new Character());

        new Repeat(3, [new Move(1), new Turn(TurnSide.Right)]).Execute(state);

        Assert.Equal(6, state.Trace.Count);
        Assert.Equal(4, state.Path.Count);
    }

    [Fact]
    public void Repeat_ZeroTimes_DoesNothing()
    {
        var state = new ExecutionState(new Character());

        new Repeat(0, [new Move(5)]).Execute(state);

        Assert.Empty(state.Trace);
        Assert.Equal(new Position(0, 0), state.Character.Position);
    }
}
