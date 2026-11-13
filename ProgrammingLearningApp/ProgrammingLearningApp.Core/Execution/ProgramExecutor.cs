using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//runs a program on a character and keeps a trace of every move and turn that happened
//the commands run themselves (Command.Execute), the executor only sets up the state
//and collects the result, so it doesnt change when a new command is added
public sealed class ProgramExecutor : IProgramRunner
{
    //runs the program on a new character that starts at (0,0) facing east
    public ExecutionResult Execute(GridProgram program) => Execute(program, new Character());

    //runs the program on a character without a grid, so no wall or edge checks
    public ExecutionResult Execute(GridProgram program, Character character) =>
        Run(program, new ExecutionState(character));

    //runs the program on a character inside a grid, walls and edges are checked on every step
    public ExecutionResult Execute(GridProgram program, Grid grid, Character character)
    {
        ArgumentNullException.ThrowIfNull(grid);

        return Run(program, new ExecutionState(character, grid));
    }

    private static ExecutionResult Run(GridProgram program, ExecutionState state)
    {
        ArgumentNullException.ThrowIfNull(program);

        foreach (var command in program.Commands)
        {
            command.Execute(state);
        }

        var character = state.Character;
        return new ExecutionResult(state.Trace, state.Path, character.X, character.Y, character.Direction);
    }
}
