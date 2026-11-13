using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Domain;

//the repeat command: runs its list of commands a number of times
public sealed class Repeat : Command
{
    private readonly List<Command> _commands;

    public int Times { get; }

    //rd only, the commands inside cant be changed from outside
    public IReadOnlyList<Command> Commands => _commands;

    public Repeat(int times, IEnumerable<Command> commands)
    {
        if (times < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(times), "A repeat must run zero or more times.");
        }

        Times = times;
        _commands = commands.ToList();
    }

    //runs the inner commands Times times, each inner command runs itself
    public override void Execute(ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        for (var i = 0; i < Times; i++)
        {
            foreach (var command in _commands)
            {
                command.Execute(state);
            }
        }
    }
}
