namespace ProgrammingLearningApp.Domain;

//base class for commands that contain other commands (Repeat and RepeatUntil)
//holds the body and runs it once, so the subclasses only decide how often the body runs
public abstract class LoopCommand : Command
{
    private readonly List<Command> _commands;

    //rd only, the commands inside cant be changed from outside
    public IReadOnlyList<Command> Commands => _commands;

    protected LoopCommand(IEnumerable<Command> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);
        _commands = commands.ToList();
    }

    //runs every command of the body one time, each command runs itself
    protected void ExecuteBody(ExecutionState state)
    {
        foreach (var command in _commands)
        {
            command.Execute(state);
        }
    }
}
