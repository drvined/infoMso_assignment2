using ProgrammingLearningApp.Domain;
using Xunit;

namespace ProgrammingLearningApp.Tests;

public class CharacterTests
{
    [Fact]
    public void NewCharacter_StartsAtOriginFacingEast()
    {
        var character = new Character();

        Assert.Equal(0, character.X);
        Assert.Equal(0, character.Y);
        Assert.Equal(Direction.East, character.Direction);
    }

    [Theory]
    [InlineData(Direction.North, Direction.East)]
    [InlineData(Direction.East, Direction.South)]
    [InlineData(Direction.South, Direction.West)]
    [InlineData(Direction.West, Direction.North)]
    public void TurnRight_GoesClockwise(Direction start, Direction expected)
    {
        var character = new Character(0, 0, start);

        character.TurnRight();

        Assert.Equal(expected, character.Direction);
    }

    [Theory]
    [InlineData(Direction.North, Direction.West)]
    [InlineData(Direction.West, Direction.South)]
    [InlineData(Direction.South, Direction.East)]
    [InlineData(Direction.East, Direction.North)]
    public void TurnLeft_GoesCounterClockwise(Direction start, Direction expected)
    {
        var character = new Character(0, 0, start);

        character.TurnLeft();

        Assert.Equal(expected, character.Direction);
    }

    // Assumption from the assignment's example: south increases Y, north decreases Y.
    [Theory]
    [InlineData(Direction.North, 0, -1)]
    [InlineData(Direction.South, 0, 1)]
    [InlineData(Direction.East, 1, 0)]
    [InlineData(Direction.West, -1, 0)]
    public void StepForward_MovesOneCellInFacingDirection(Direction direction, int x, int y)
    {
        var character = new Character(0, 0, direction);

        character.StepForward();

        Assert.Equal(x, character.X);
        Assert.Equal(y, character.Y);
    }

    [Fact]
    public void Turning_DoesNotChangePosition()
    {
        var character = new Character(3, 4, Direction.East);

        character.TurnLeft();
        character.TurnRight();
        character.TurnRight();

        Assert.Equal(3, character.X);
        Assert.Equal(4, character.Y);
    }
}
