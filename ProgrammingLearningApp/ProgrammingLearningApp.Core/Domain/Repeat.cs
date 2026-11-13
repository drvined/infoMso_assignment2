namespace ProgrammingLearningApp.Domain;

//the repeat command: runs its list of commands a number of times
public sealed class Repeat : LoopCommand
{
    public int Times { get; }

    public Repeat(int times, IEnumerable<Command> commands) : base(commands)
    {
        if (times < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(times), "A repeat must run zero or more times.");
        }

        Times = times;
    }

    //runs the inner commands Times times
    public override void Execute(ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        for (var i = 0; i < Times; i++)
        {
            ExecuteBody(state);
        }
    }
}
