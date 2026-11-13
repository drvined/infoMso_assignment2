using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//everything a command needs while a program runs: the character, the trace of
//executed commands and the path (every cell the character has been on)
//commands get this passed into Execute so they dont need to know about the executor
//(not called ExecutionContext because that name already exists in System.Threading)
public sealed class ExecutionState
{
    private readonly List<string> _trace = [];
    private readonly List<Position> _path = [];

    public Character Character { get; }

    public IReadOnlyList<string> Trace => _trace;

    //starts with the start cell, then one entry per step
    public IReadOnlyList<Position> Path => _path;

    public ExecutionState(Character character)
    {
        ArgumentNullException.ThrowIfNull(character);

        Character = character;
        _path.Add(character.Position);
    }

    //moves the character one cell and remembers it in the path
    //this is the one place every step goes through, so the grid checks
    //(blocked cell / outside the grid) can be added here later
    public void StepForward()
    {
        Character.StepForward();
        _path.Add(Character.Position);
    }

    public void AddToTrace(string entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        _trace.Add(entry);
    }
}
