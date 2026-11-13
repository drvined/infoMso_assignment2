using ProgrammingLearningApp.Domain;
using ProgrammingLearningApp.Execution;

namespace ProgrammingLearningApp.Tests;

//a fake runner: it does not run the program, it returns the result or the error the test chooses,
//and it remembers what it was asked to run
public sealed class FakeProgramRunner : IProgramRunner
{
    private readonly ExecutionResult? _result;
    private readonly ProgramRuntimeException? _error;

    public Grid? LastGrid { get; private set; }

    public Position LastStart { get; private set; }

    public FakeProgramRunner(ExecutionResult result)
    {
        _result = result;
    }

    public FakeProgramRunner(ProgramRuntimeException error)
    {
        _error = error;
    }

    public ExecutionResult Execute(GridProgram program, Grid grid, Character character)
    {
        LastGrid = grid;
        LastStart = character.Position;

        if (_error is not null)
        {
            throw _error;
        }

        return _result!;
    }
}
