namespace ProgrammingLearningApp.Domain;

//a yes or no question about the current state, used by commands that need a check
public abstract class Condition
{
    public abstract bool IsTrue(ExecutionState state);
}