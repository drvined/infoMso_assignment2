using ProgrammingLearningApp.Domain;

namespace ProgrammingLearningApp.Execution;

//runs a program on a character and keeps a trace of every move and turn that happened
//the commands run themselves (Command.Execute), the executor only sets up the state
//and collects the result, so it doesnt change when a new command is added
public sealed class ProgramExecutor
{
    //runs the program on a new character that starts at (0,0) facing east
    public ExecutionResult Execute(GridProgram program) => Execute(program, new Character());

    //runs the program on a character, that character gets changed
    //at the end the trace, the path and where the character ended up go into ExecutionResult
    public ExecutionResult Execute(GridProgram program, Character character)
    {
        ArgumentNullException.ThrowIfNull(program);
        ArgumentNullException.ThrowIfNull(character);

        var state = new ExecutionState(character);

        foreach (var command in program.Commands)
        {
            command.Execute(state);
        }

        return new ExecutionResult(state.Trace, state.Path, character.X, character.Y, character.Direction);
    }
}
