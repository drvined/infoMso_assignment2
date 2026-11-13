namespace ProgrammingLearningApp.Domain;

//this the whole program so a name and the list of commands at the top level
//repeats keep their own commands inside them so this only holds the outer ones
public sealed class GridProgram
{
    private readonly List<Command> _commands;

    public string Name { get; }

    public IReadOnlyList<Command> Commands => _commands;

    //makes a copy of the commands so changing the original list later doesnt change the program
    public GridProgram(string name, IEnumerable<Command> commands)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(commands);

        Name = name;
        _commands = commands.ToList();
    }
}