namespace ProgrammingLearningApp.Domain;

//the repeat until command runs its commands again and again until the condition is true
//the condition is checked before every round, so when it is already true the commands never run
public sealed class RepeatUntil : Command
{
    private readonly List<Command> _commands;

    public Condition Condition { get; }

    public IReadOnlyList<Command> Commands => _commands;

    public RepeatUntil(Condition condition, IEnumerable<Command> commands)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        _commands = commands.ToList();
    }

    public override void Execute(ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        while (!Condition.IsTrue(state))
        {
            state.CountStep();

            foreach (var command in _commands)
            {
                command.Execute(state);
            }
        }
    }
}