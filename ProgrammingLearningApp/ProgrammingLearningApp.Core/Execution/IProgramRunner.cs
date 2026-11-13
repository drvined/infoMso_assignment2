using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//runs a program on a grid code that needs a run depends on this interface
//so tests can replace the real executor with a fake
public interface IProgramRunner
{
    ExecutionResult Execute(GridProgram program, Grid grid, Character character);
}
