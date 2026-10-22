namespace ProgrammingLearningApp.Domain;

//the move command:  the character walks this many steps forward in the direction its facing
public sealed class Move : Command
{
    public int Steps { get; }

    public Move(int steps)
    {
        if (steps < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(steps), "A move must be zero or more steps.");
        }

        Steps = steps;
    }
}