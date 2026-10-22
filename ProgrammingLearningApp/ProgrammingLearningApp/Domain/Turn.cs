namespace ProgrammingLearningApp.Domain;

public enum TurnSide
{
    Left,
    Right
}

public sealed class Turn : Command
{
    public TurnSide Side { get; }

    public Turn(TurnSide side)
    {
        Side = side;
    }
}