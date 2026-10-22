namespace ProgrammingLearningApp.Domain;
public sealed class Character
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Direction Direction { get; private set; }

    public Character() : this(0, 0, Direction.East)
    {
    }

    public Character(int x, int y, Direction direction)
    {
        X = x;
        Y = y;
        Direction = direction;
    }

    public void TurnLeft() => Direction = Direction.TurnLeft();

    public void TurnRight() => Direction = Direction.TurnRight();

    public void StepForward()
    {
        switch (Direction)
        {
            case Direction.North: Y--; break;
            case Direction.South: Y++; break;
            case Direction.East: X++; break;
            case Direction.West: X--; break;
        }
    }
}