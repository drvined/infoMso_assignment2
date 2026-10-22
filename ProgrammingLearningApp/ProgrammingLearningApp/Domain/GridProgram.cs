namespace ProgrammingLearningApp.Domain;
public sealed class GridProgram
{
    private readonly List<Command> _commands;

    public string Name { get; }

    public IReadOnlyList<Command> Commands => _commands;

    public GridProgram(string name, IEnumerable<Command> commands)
    {
        Name = name;
        _commands = commands.ToList();
    }
}