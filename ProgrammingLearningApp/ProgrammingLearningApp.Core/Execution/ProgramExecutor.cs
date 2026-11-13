using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//runs a program on character and keeps a trace of every move and turn that happened
public sealed class ProgramExecutor : IProgramRunner
{
    //runs program on new character that starts at 0,0 facing east
    public ExecutionResult Execute(GridProgram program) => Execute(program, new Character());

    //runs the program character without grid
    public ExecutionResult Execute(GridProgram program, Character character) =>
        Run(program, new ExecutionState(character));

    //runs  program on character inside grid
    public ExecutionResult Execute(GridProgram program, Grid grid, Character character)
    {
        ArgumentNullException.ThrowIfNull(grid);

        return Run(program, new ExecutionState(character, grid));
    }

    private static ExecutionResult Run(GridProgram program, ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(program);

        try
        {
            foreach (var command in program.Commands)
            {
                command.Execute(state);
            }
        }
        catch (ProgramRuntimeException ex)
        {
            ex.Path = state.Path.ToList();
            ex.Trace = state.Trace.ToList();
            throw;
        }

        var character = state.Character;
        return new ExecutionResult(state.Trace, state.Path, character.X, character.Y, character.Direction);
    }
}
