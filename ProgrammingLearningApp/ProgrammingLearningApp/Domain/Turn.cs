namespace ProgrammingLearningApp.Domain;

//which way to turn an enum so only left or right is pssible and no typos
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