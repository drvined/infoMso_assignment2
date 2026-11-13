namespace ProgrammingLearningApp.Domain;

//the figure that walks over the grid keep track of where it is and which way it faces
public sealed class Character
{
    public int X { get; private set; }
    public int Y { get; private set; }
    public Direction Direction { get; private set; }

    public Position Position => new(X, Y);

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

    //movs one cell in the direction its facing
    //y goes down when going north and up when going south
    public void StepForward()
    {
        var next = Position.Step(Direction);
        X = next.X;
        Y = next.Y;
    }
}