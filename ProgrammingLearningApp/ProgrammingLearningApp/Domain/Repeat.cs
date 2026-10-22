namespace ProgrammingLearningApp.Domain;
public sealed class Repeat : Command
{
    private readonly List<Command> _commands;

    public int Times { get; }

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
}