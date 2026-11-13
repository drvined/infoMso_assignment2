namespace ProgrammingLearningApp.Domain;

//a yes or no question about the current state, used by commands that need a check
public abstract class Condition
{
    //the name used in the program text, for example WallAhead
    public abstract string Name { get; }

    public abstract bool IsTrue(ExecutionState state);
}