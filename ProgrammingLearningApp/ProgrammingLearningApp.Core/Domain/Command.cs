using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Domain;

//base class for every command in the language
public abstract class Command
{
    public abstract void Execute(ExecutionState state);
}
