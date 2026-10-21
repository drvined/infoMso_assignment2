namespace ProgrammingLearningApp.Domain;

public enum Direction
{
    North,
    East,
    South,
    West
}

public static class DirectionExtensions
{
    public static Direction TurnRight(this Direction direction) =>
        (Direction)(((int)direction + 1) % 4);

    public static Direction TurnLeft(this Direction direction) =>
        (Direction)(((int)direction + 3) % 4);
}