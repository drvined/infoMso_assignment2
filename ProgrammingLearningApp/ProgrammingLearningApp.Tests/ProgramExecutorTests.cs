using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class ProgramExecutorTests
{
    private static ExecutionResult Run(string text) =>
        new ProgramExecutor().Execute(TestPrograms.Parse(text));

    [Fact]
    public void Example1_TraceAndEndState()
    {
        ExecutionResult result = Run(TestPrograms.Example1);

        Assert.Equal(
            "Move 10, Turn right, Move 10, Turn right, Move 10, Turn right, Move 10, Turn right.",
            result.FormatTrace());
        Assert.Equal("End state (0,0) facing east.", result.FormatEndState());
    }

    [Fact]
    public void Example2_GivesSameResultAsExample1()
    {
        ExecutionResult result = Run(TestPrograms.Example2);

        Assert.Equal(
            "Move 10, Turn right, Move 10, Turn right, Move 10, Turn right, Move 10, Turn right.",
            result.FormatTrace());
        Assert.Equal("End state (0,0) facing east.", result.FormatEndState());
    }

    [Fact]
    public void Example3_EndsAtExpectedPosition()
    {
        ExecutionResult result = Run(TestPrograms.Example3);

        Assert.Equal(3, result.FinalX);
        Assert.Equal(10, result.FinalY);
        Assert.Equal(Direction.South, result.FinalDirection);
    }

    [Fact]
    public void Move_East_IncreasesX()
    {
        ExecutionResult result = Run("Move 4");

        Assert.Equal(4, result.FinalX);
        Assert.Equal(0, result.FinalY);
    }

    [Fact]
    public void MoveZero_LeavesCharacterInPlace()
    {
        ExecutionResult result = Run("Move 0");

        Assert.Equal("Move 0.", result.FormatTrace());
        Assert.Equal(0, result.FinalX);
        Assert.Equal(0, result.FinalY);
    }

    [Fact]
    public void TurnLeftFromEast_FacesNorth()
    {
        ExecutionResult result = Run("Turn left");

        Assert.Equal(Direction.North, result.FinalDirection);
    }
}